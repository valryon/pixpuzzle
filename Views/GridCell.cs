using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace PixPuzzle
{
	public struct CellColor {
		public float A;
		public float R;
		public float G;
		public float B;


		public override bool Equals (object obj)
		{
			if(obj is CellColor) {
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

		public GridCell (RectangleF frame)
			: base(frame)
		{
			// Create label
			label = new UILabel(new RectangleF(0,0,frame.Width,frame.Height));
			label.Hidden = false;
			label.TextColor = UIColor.Black;
			label.Text = "*";
			label.TextAlignment = UITextAlignment.Center;
			label.Layer.BorderColor = UIColor.Black.CGColor;
			label.Layer.BorderWidth = 1;

			AddSubview(label);

			SetColor(new CellColor());
		}

		public void SetValue(string val) {
			label.Text = val;
			//label.BackgroundColor = UIColor.LightGray;
		}

		public void IsTextDisplayed(bool display) {
			label.Hidden = !display;
		}

		public void SetColor(CellColor color) {
			this.color = color;

			UIColor uiColor  = new UIColor(color.R, color.G, color.B, color.A);
			label.TextColor = uiColor;
			//label.BackgroundColor = uiColor;
		}

		public CellColor Color
		{
			get {
				return color;
			}
		}

		public bool IsMarked
		{
			get;set;
		}
	}
}

