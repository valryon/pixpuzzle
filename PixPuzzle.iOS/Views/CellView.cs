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
			// TODO Refactoring
			if (Path == null) {
				// The empty style
				label.TextColor = UIColor.Black;
				label.BackgroundColor = defaultCellBackgroundColor;
				label.Text = "";
			} else {
				bool validPath = Path.IsValid;

				// Style for path part
				if (IsPathStartOrEnd == false) {
					label.TextColor = UIColor.LightGray;
					label.Text = Path.Length.ToString ();
					label.BackgroundColor = Path.Color.UIColor;
				} else {
					//Style for end and starts 
					label.TextColor = UIColor.LightGray;
					label.BackgroundColor = Path.Color.UIColor;
					label.Layer.CornerRadius = 35;
				}

				if (validPath) {
					label.BackgroundColor = Path.Color.UIColor;
					label.TextColor = UIColor.Yellow;
					this.label.Text = Path.Length.ToString ();
					this.label.Layer.CornerRadius = 0;
				} else {
					label.BackgroundColor = defaultCellBackgroundColor;
				}
			}
		}

		public override void SelectCell ()
		{
			View.Transform = CGAffineTransform.MakeScale (0.85f, 0.85f);
			
			UIView.Animate (0.5f,
			                () => {
				View.Transform = CGAffineTransform.MakeScale (1f, 1f);					
			});
		}

		public override void UnselectCell (bool success)
		{
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

