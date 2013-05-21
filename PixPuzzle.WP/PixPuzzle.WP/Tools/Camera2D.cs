using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixPuzzle.Data.WP
{
    /// <summary>
    /// A 2D camera that can move, zoom, fade in and out, shake...
    /// </summary>
    public class Camera2D
    {
        public float Rotation { get; set; }
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }

        public Matrix Transform { get; private set; }
        public Rectangle VisibilityRect { get; private set; }
        public Viewport Viewport { get; private set; }

        public Camera2D(Viewport viewport)
        {
            Transform = Matrix.Identity;
            Viewport = viewport;

            Zoom = 1.0f;
            Rotation = 0f;
            Position = Vector2.Zero;
        }

        public void Update()
        {
            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                        Matrix.CreateTranslation(Viewport.Width / 2, Viewport.Height / 2, 0);

            VisibilityRect = new Rectangle
            {
                X = (int)(Position.X - ((Viewport.Width / 2) / Zoom)),
                Y = (int)(Position.Y - ((Viewport.Height / 2) / Zoom)),
                Width = (int)(Viewport.Width / Zoom),
                Height = (int)(Viewport.Height / Zoom)
            };
        }

        #region Coordinates utilities

        /// <summary>
        /// Transform a Camera position into a world position
        /// </summary>  
        /// <remarks>Example: Mouse pointer on the screen to world coordinates</remarks>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToWorldLocation(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(Transform));
        }

        /// <summary>
        /// Transform a world position into a camera one
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 ToScreenLocation(Vector2 position)
        {
            return Vector2.Transform(position, Transform);
        }

        #endregion
    }
}
