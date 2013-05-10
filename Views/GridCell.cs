using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace PixPuzzle
{
	public class GridCell : UIView
	{
		private UIColor color;
		private UILabel label;

		public GridCell (RectangleF frame)
			: base(frame)
		{
			// Create label
			label = new UILabel(new RectangleF(0,0,frame.Width,frame.Height));
			label.Hidden = false;
			label.TextColor = UIColor.Black;
			label.Text = "9";
			label.TextAlignment = UITextAlignment.Center;
			label.Layer.BorderColor = UIColor.Black.CGColor;
			label.Layer.BorderWidth = 1;

			AddSubview(label);

			SetColor(UIColor.Black);
		}

		public void SetColor(UIColor color) {
			this.color = color;
			label.TextColor = color;
		}
	}
}

