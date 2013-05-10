using System;
using MonoTouch.UIKit;
using System.Drawing;

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
	}

	public class GridCell : UIView
	{
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
			label.TextColor = UIColor.Black;
			label.Text = "*";
			label.TextAlignment = UITextAlignment.Center;

			// The border
			this.Layer.BorderColor = UIColor.Black.CGColor;
			this.Layer.BorderWidth = 1;

			AddSubview (label);

			IsTextDisplayed  = false;
			Color = new CellColor();
		}

		public void SetCount (int val)
		{
			label.Text = val.ToString ();
			//label.BackgroundColor = UIColor.LightGray;
		}

		public bool IsTextDisplayed {
			get {
				return !label.Hidden;
			}
			set {
				label.Hidden = !value;
			}
		}

		public int X {
			get;
			private set;
		}

		public int Y {
			get;
			private set;
		}

		public CellColor Color {
			get {
				return color;
			}
			set {
				this.color = value;
				
				UIColor uiColor = new UIColor (color.R, color.G, color.B, color.A);
				label.TextColor = uiColor;
			}
		}

		public bool IsMarked {
			get;
			set;
		}
	}
}

