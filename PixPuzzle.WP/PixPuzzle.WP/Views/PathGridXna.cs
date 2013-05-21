using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using PixPuzzle.Data;
using PixPuzzle.Data.WP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixPuzzle.WP.Views
{
    /// <summary>
    /// The grid is made of XNA
    /// </summary>
    public class PathGridXna : PathGrid, IGridView, IDisposable
    {
        private const int defaultCellSize = 64;

        private Camera2D camera;
        private GamePage parent;
        private SpriteBatch spriteBatch;
        private Rectangle gridRect;

        // Inputs
        private TouchCollection previousInputState, inputState;
        private bool isTouching;

        public PathGridXna(GamePage parent, int width, int height)
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

            TouchPanel.EnabledGestures = GestureType.DoubleTap | GestureType.Pinch;
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

            handleInputs();
        }

        private void handleInputs()
        {
            // Inputs
            previousInputState = inputState;
            inputState = TouchPanel.GetState();

            // Do things with fingers
            if (inputState.Count > 0)
            {
                isTouching = true;

                // Gesttres
                while (TouchPanel.IsGestureAvailable)
                {
                    var gesture = TouchPanel.ReadGesture();

                    // Delete path
                    if (gesture.GestureType == GestureType.DoubleTap)
                    {
                        PathCell cell = getCellFromScreenPosition(gesture.Position);

                        RemovePath(cell);
                    }
                    else if (gesture.GestureType == GestureType.Pinch)
                    {
                        // TODO zoom
                    }
                }

                if (previousInputState.Count > 0)
                {
                    // Moving
                    if (inputState.Count == 2)
                    {
                        Vector2 movement = inputState[0].Position - previousInputState[0].Position;

                        camera.Position -= movement;
                    }
                    // Playing
                    else if (inputState.Count == 1)
                    {
                        TouchLocation touch = inputState[0];

                        PathCell cell = getCellFromScreenPosition(touch.Position);

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

        private PathCell getCellFromScreenPosition(Vector2 screenposition)
        {
            Vector2 gridLocation = camera.ToWorldLocation(screenposition);
            int x = (int)(gridLocation.X / (float)CellSize);
            int y = (int)(gridLocation.Y / (float)CellSize);

            PathCell cell = GetCell(x, y);

            return cell;
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
            Color gridBorderColor = Color.MediumVioletRed;
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Left, gridRect.Top, gridRect.Width, BorderWidth), gridBorderColor);
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Left, gridRect.Bottom, gridRect.Width, BorderWidth), gridBorderColor);
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Left, gridRect.Top, BorderWidth, gridRect.Height), gridBorderColor);
            spriteBatch.Draw(parent.BlankTexture, new Rectangle(gridRect.Right, gridRect.Top, BorderWidth, gridRect.Height), gridBorderColor);

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
}
