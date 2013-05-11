using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace PixPuzzle
{
	public struct CellColor
	{
		public float A;
		public float R;
		public float G;
		public float B;

		public CellColor (float a, float r, float g, float b)
		{
			A = a;
			R = r;
			G = g;
			B = b;
		}

		public override bool Equals (object obj)
		{
			if (obj is CellColor) {
				CellColor otherColor = (CellColor)obj;

				return A == otherColor.A && R == otherColor.R && G == otherColor.G && B == otherColor.B;
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public UIColor UIColor {
			get {
				return new UIColor (R, G, B, A);
			}
		}
	}

	public class GridCell : UIView
	{
		// Common to all cells
		private CellColor color;
		private UILabel label;

		public GridCell (int x, int y, RectangleF frame)
			: base(frame)
		{
			X = x;
			Y = y;

			// Create label
			label = new UILabel (new RectangleF (0, 0, frame.Width, frame.Height));
			label.Hidden = false;
			label.BackgroundColor = UIColor.FromRGB (230, 230, 230);
			label.TextColor = UIColor.Black;
			label.Text = "";
			label.TextAlignment = UITextAlignment.Center;

			// The border
			this.Layer.BorderColor = UIColor.Black.CGColor;
			this.Layer.BorderWidth = 1;

			AddSubview (label);

			// Default values
			IsPathStartOrEnd = false;
		}
		/// <summary>
		/// Sets the color for this cell from the pixel data of the image
		/// </summary>
		/// <param name="color">Color.</param>
		public void DefineBaseColor (CellColor color)
		{
			Color = color;

			UIColor uiColor = Color.UIColor;
			label.TextColor = uiColor;
		}
		/// <summary>
		/// Sets the number to display. It also means that the cell is a path start or end.
		/// </summary>
		/// <param name="val">Value.</param>
		public void DefineCellAsPathStartOrEnd (int pathLength)
		{
			IsPathStartOrEnd = true;
			label.Text = pathLength.ToString ();

			// The cell is the beginning or the end of a path
			DefinePath (new Path(this, pathLength));
		}
		/// <summary>
		/// Mark the cell as being in a complete path
		/// </summary>
		public void MarkComplete ()
		{
			label.BackgroundColor = color.UIColor;
			label.TextColor = UIColor.Yellow;
		}
		/// <summary>
		/// The cell isn't in a valid path anymore
		/// </summary>
		public void UnmarkComplete ()
		{
			label.BackgroundColor = UIColor.White;
			label.TextColor = color.UIColor;
		}
		#region Events

		/// <summary>
		/// Cell has been selected (touched)
		/// </summary>
		public void SelectCell ()
		{
			this.Transform = CGAffineTransform.MakeScale (0.85f, 0.85f);

			UIView.Animate (0.5f,
			                () => {
				this.Transform = CGAffineTransform.MakeScale (1f, 1f);					
			});
		}
		/// <summary>
		/// Touch released
		/// </summary>
		public void UnselectCell (bool success)
		{
			if (success) {
				this.Transform = CGAffineTransform.MakeScale (1.25f, 1.25f);

				UIView.Animate (0.5f,
				               () => {
					this.Transform = CGAffineTransform.MakeScale (1f, 1f);					
				});
			}
		}
		/// <summary>
		/// Define the path where the cell is included
		/// </summary>
		/// <param name="p">P.</param>
		public void DefinePath (Path p)
		{
			Path = p;

			// Update the cell view
			if (IsPathStartOrEnd == false) {
				this.label.BackgroundColor = Path.Color.UIColor;
			} 
			// TODO Style for end and starts
		}
		#endregion

		#region Properties

		/// <summary>
		/// Tells if we are on a cell that is the start or the end of a complete path
		/// </summary>
		/// <value><c>true</c> if this instance is path start or end; otherwise, <c>false</c>.</value>
		public bool IsPathStartOrEnd {
			get;
			private set;
		}
		/// <summary>
		/// Cell path
		/// </summary>
		/// <value>The path.</value>
		public Path Path {
			get;
			private set;
		}
		/// <summary>
		/// Location (X)
		/// </summary>
		/// <value>The x.</value>
		public int X {
			get;
			private set;
		}
		/// <summary>
		/// Location (Y)
		/// </summary>
		/// <value>The y.</value>
		public int Y {
			get;
			private set;
		}
		/// <summary>
		/// Set the right color form the image
		/// </summary>
		/// <value>The color.</value>
		public CellColor Color {
			get;
			private set;
		}
		/// <summary>
		/// Cells has been marked by grid creator
		/// </summary>
		/// <value><c>true</c> if this instance is marked; otherwise, <c>false</c>.</value>
		public bool IsMarked {
			get;
			set;
		}
		#endregion
	}
}

