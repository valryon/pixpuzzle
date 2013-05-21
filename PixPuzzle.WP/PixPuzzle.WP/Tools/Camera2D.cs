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
        public float Rotation {get;set;}
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }

        public Matrix Transform { get; private set; }
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
        }
    }
}
