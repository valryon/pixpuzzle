using System;
using MonoTouch.UIKit;

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
			label = new UILabel(frame);
			label.Hidden = false;
			label.TextColor = UIColor.Black;
			label.Text = "9";

			AddSubview(label);

			SetColor(UIColor.Black);
		}

		public void SetColor(UIColor color) {
			this.color = color;
			label.TextColor = color;
		}
	}
}

