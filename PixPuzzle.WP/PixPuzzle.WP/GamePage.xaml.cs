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
using PixPuzzle.WP.Views;

namespace PixPuzzle.WP
{

    public partial class GamePage : PhoneApplicationPage
    {
        private ContentManager contentManager;
        private GameTimer timer;
        private SpriteBatch spriteBatch;

        private PathGridXna grid;

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

            grid = new PathGridXna(this, imageWidth, imageHeight);
            grid.GridCompleted += grid_GridCompleted;
            grid.CreateGrid(0, 0, grid);
            grid.LoadContent(contentManager);

            CellColor[][] colors = new CellColor[imageWidth][];

            for (int x = 0; x < imageWidth; x++)
            {
                colors[x] = new CellColor[imageHeight];

                for (int y = 0; y < imageHeight; y++)
                {
                    CellColor c;
                    c = new CellColor()
                    {
                        A = colors2D[x, y].A / 255f,
                        R = colors2D[x, y].R / 255f,
                        G = colors2D[x, y].G / 255f,
                        B = colors2D[x, y].B / 255f
                    };

                    colors[x][y] = c;
                }
            }

            grid.SetupGrid(colors);
        }

        void grid_GridCompleted()
        {
            MessageBox.Show("Bravo, tu as fini le puzzle ! Tu peux tuer l'appli maintenant, il n'y a rien après.", "Game Over", MessageBoxButton.OK);
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
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.LightBlue);

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