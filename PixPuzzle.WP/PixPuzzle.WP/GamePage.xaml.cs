using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixPuzzle.Data;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using PixPuzzle.Data.WP;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace PixPuzzle.WP
{
    /// <summary>
    /// The grid is made of XNA
    /// </summary>
    public class GridXna : Grid, IGridView, IDisposable
    {
        private const int defaultCellSize = 48;

        private Camera2D camera;
        private GamePage parent;
        private SpriteBatch spriteBatch;
        private Rectangle gridRect;

        // Inputs
        private TouchCollection previousInputState, inputState;
        private bool isTouching;

        public GridXna(GamePage parent, int width, int height)
            : base(width, height, defaultCellSize)
        {
            this.parent = parent;

            this.camera = new Camera2D(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport);
        }

        public void InitializeViewForDrawing()
        {
            gridRect = new Rectangle(GridLocation.X, GridLocation.Y
                                 , (CellSize * Width) + GridLocation.X + BorderWidth
                                 , (CellSize * Height) + GridLocation.Y + BorderWidth
                                 );

            camera.Position = new Vector2(gridRect.Center.X, gridRect.Center.Y);
        }

        public void OrderRefresh(Rectangle zoneToRefresh)
        {
            // Nothing to do in XNA, as we draw the grid every frame
        }

        public void LoadContent(ContentManager content)
        {

        }

        public void Update(GameTimerEventArgs gameTime)
        {
            camera.Update();

            // Inputs
            previousInputState = inputState;
            inputState = TouchPanel.GetState();

            // Do things with fingers
            if (inputState.Count > 0)
            {
                isTouching = true;

                if (previousInputState.Count > 0)
                {
                    // Moving
                    if (inputState.Count == 2)
                    {
                        Vector2 movement = inputState[0].Position - previousInputState[0].Position;

                        camera.Position += new Vector2(-movement.X, movement.Y);
                    }
                    // Playing
                    else if (inputState.Count == 1)
                    {
                        TouchLocation touch = inputState[0];

                        Vector2 gridLocation = camera.ToWorldLocation(touch.Position);
                        int x = (int)(gridLocation.X / (float)CellSize);
			            int y = (int)(gridLocation.Y / (float)CellSize);

                        Cell cell = GetCell(x, y);

                        if (IsCreatingPath == false)
                        {
                            StartPathCreation(cell);
                        }
                        else
                        {
                            CreatePath(cell);
                        }
                    }
                }
            }
            else
            {
                if (isTouching)
                {
                    EndPathCreation();
                    isTouching = false;
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            // Draw the grid
            DrawPuzzle();
        }

        public void StartDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, this.camera.Transform);
        }

        public bool IsToRefresh(Cell cell, Rectangle cellRect)
        {
            return camera.VisibilityRect.Intersects(cellRect) || camera.VisibilityRect.Contains(cellRect);
        }

        public void DrawGrid()
        {
            // Background
            spriteBatch.Draw(parent.BlankTexture, gridRect, Color.LightGray);

            // Draw the borders of the grid
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Left, gridRect.Top, gridRect.Width, BorderWidth), Color.Blue);
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Left, gridRect.Bottom, gridRect.Width, BorderWidth), Color.Blue);
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Left, gridRect.Top, BorderWidth, gridRect.Height), Color.Blue);
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Right, gridRect.Top, BorderWidth, gridRect.Height), Color.Blue);

            // Draw cells 
            for (int x = 0; x < Width; x++)
            {
                int cellBorderX = BorderStartLocation.X + x * CellSize;

                Rectangle dRect = new Rectangle(cellBorderX, BorderStartLocation.Y, BorderWidth, BorderStartLocation.Y + Height * CellSize);

                spriteBatch.Draw(parent.BlankTexture, dRect, Color.Black);
            }

            for (int y = 0; y < Height; y++)
            {
                int cellBorderY = BorderStartLocation.Y + y * CellSize;

                Rectangle dRect = new Rectangle(BorderStartLocation.X, cellBorderY, BorderStartLocation.X + Width * CellSize, BorderWidth);

                spriteBatch.Draw(parent.BlankTexture, dRect, Color.Black);
            }
        }

        public void DrawCellBase(Rectangle rectangle, bool isValid, bool isPathEndOrStart, CellColor cellColor)
        {
            if (isValid)
            {
                spriteBatch.Draw(parent.BlankTexture, rectangle, Color.Blue);
            }
            rectangle.Inflate(-CellSize / 8, -CellSize / 8);
            spriteBatch.Draw(parent.BlankTexture, rectangle, cellColor.ToXnaColor());
        }

        public void DrawPath(Rectangle pathRect, Microsoft.Xna.Framework.Point direction, CellColor color)
        {
            spriteBatch.Draw(parent.BlankTexture, pathRect, color.ToXnaColor());
        }

        public void DrawLastCellIncompletePath(Rectangle rect, string pathValue, CellColor color)
        {
            Vector2 cellCenter = new Vector2(rect.Center.X, rect.Center.Y);

            int size = CellSize / 4;
            Rectangle smallRect = new Rectangle((int)cellCenter.X - size, (int)cellCenter.Y - size, 2 * size, 2 * size);
            spriteBatch.Draw(parent.BlankTexture, smallRect, Color.LightSalmon);

            spriteBatch.DrawString(parent.Font, pathValue, cellCenter - (parent.Font.MeasureString(pathValue) / 2), Color.Black);
        }

        public void DrawEndOrStartText(Rectangle location, string text, CellColor color)
        {
            float rgb = color.B + color.R + color.G;

            Color textColor = Color.Black;
            Vector2 cellCenter = new Vector2(location.Center.X, location.Center.Y);

            if (rgb < 0.25f)
            {
                textColor = Color.White;
            }

            spriteBatch.DrawString(parent.Font, text, cellCenter - (parent.Font.MeasureString(text) / 2), textColor);
        }

        public void EndDraw()
        {
            spriteBatch.End();
        }

        public void Dispose()
        {
            parent = null;
        }
    }

    public partial class GamePage : PhoneApplicationPage
    {
        private ContentManager contentManager;
        private GameTimer timer;
        private SpriteBatch spriteBatch;

        private GridXna grid;

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;
        }

        private void loadContent(string image)
        {
            // Create an empty texture
            BlankTexture = new Texture2D(SharedGraphicsDeviceManager.Current.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] color = new Color[1] { Color.White };
            BlankTexture.SetData(color);

            // Load font
            Font = contentManager.Load<SpriteFont>("font");

            // Read image
            Texture2D puzzleImage = contentManager.Load<Texture2D>(image);

            // Width and height
            int imageWidth = puzzleImage.Width;
            int imageHeight = puzzleImage.Height;

            // Colors pixel by pixel
            Color[] colors1D = new Color[puzzleImage.Width * puzzleImage.Height];
            puzzleImage.GetData(colors1D);
            Color[,] colors2D = new Color[puzzleImage.Width, puzzleImage.Height];
            for (int x = 0; x < puzzleImage.Width; x++)
            {
                for (int y = 0; y < puzzleImage.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * puzzleImage.Width];
                }
            }

            grid = new GridXna(this, imageWidth, imageHeight);
            grid.CreateGrid(0, 0, grid);
            grid.LoadContent(contentManager);

            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    CellColor c;

                    //if (colors2D[x, y].A < 20)
                    if (colors2D[x, y].A > 25f)
                    {
                        c = new CellColor()
                        {
                            A = colors2D[x, y].A / 255f,
                            R = colors2D[x, y].R / 255f,
                            G = colors2D[x, y].G / 255f,
                            B = colors2D[x, y].B / 255f
                        };
                    }
                    else
                    {
                        c = new CellColor()
                        {
                            A = 1f,
                            R = 1f,
                            G = 1f,
                            B = 1f
                        };
                    }

                    grid.SetPixelData(x, y, c);
                }
            }

            grid.SetupGrid();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            loadContent("0");

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            grid.Update(e);
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

            grid.Draw(spriteBatch);
        }

        public SpriteFont Font
        {
            get;
            private set;
        }

        public Texture2D BlankTexture
        {
            get;
            private set;
        }
    }
}