using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using PixPuzzle.Data;

namespace PixPuzzle
{
	/// <summary>
	/// iOS View for cell
	/// </summary>
	public class CellView : Cell
	{
		private static UIColor defaultCellBackgroundColor = UIColor.FromRGB (230, 230, 230);

		private UILabel label;
		private RectangleF frame;

		public CellView (int x,int y, RectangleF frame)
			: base(x,y)
		{
			this.frame = frame;
		}

		#region implemented abstract members of Cell

		public override void BuildView ()
		{
			this.View = new UIView (frame);

			// Create label
			label = new UILabel (new RectangleF (0, 0, frame.Width, frame.Height));
			label.Hidden = false;
			label.TextAlignment = UITextAlignment.Center;

			// The border
			View.Layer.BorderColor = UIColor.Black.CGColor;
			View.Layer.BorderWidth = 1;

			// Performance improvement (I guess)
			View.Layer.MasksToBounds = false;
			View.Layer.ShouldRasterize = true;

			View.AddSubview (label);
		}

		public override void UpdateView ()
		{
			label.Text = string.Empty;

			if (Path == null) {

				// The empty style
				label.TextColor = UIColor.Black;
				label.BackgroundColor = defaultCellBackgroundColor;

			} else {

				label.BackgroundColor = Path.Color.UIColor;

				bool validPath = Path.IsValid;

				// The path is valid: the background is filled
				if (validPath) {
					label.TextColor = UIColor.Yellow;
					label.Layer.CornerRadius = 0;
				}

				// End or start: Display the number
				if (IsPathStartOrEnd) {
					label.Text = Path.ExpectedLength.ToString ();

					if (validPath == false) {
						label.Layer.CornerRadius = 15;
						label.TextColor = UIColor.Black;
					}
				}

				// Draw the path

				// -- Previous cell
				Cell previousCell = Path.PreviousCell(this);
				if (previousCell != null) {

				} else {

				}

				// Next cell
				Cell nextCell= Path.NextCell(this);
				if (nextCell != null) {

				} else {
					// No next cell?
					// Display the current path length
					// Except for starts or ends
					if (IsPathStartOrEnd == false) {
						label.TextColor = UIColor.LightGray;
						label.Text = Path.Length.ToString ();
					}

				}



			}
		}

		public override void SelectCell ()
		{
			base.SelectCell ();

			View.Transform = CGAffineTransform.MakeScale (0.85f, 0.85f);
			
			UIView.Animate (0.5f,
			                () => {
				View.Transform = CGAffineTransform.MakeScale (1f, 1f);					
			});
		}

		public override void UnselectCell (bool success)
		{
			base.UnselectCell (success);

			if (success) {
				View.Transform = CGAffineTransform.MakeScale (1.25f, 1.25f);
				
				UIView.Animate (0.5f,
				                    () => {
					View.Transform = CGAffineTransform.MakeScale (1f, 1f);					
				});
			}
		}
		#endregion

		public UIView View {
			get;
			set;
		}
	}
}

