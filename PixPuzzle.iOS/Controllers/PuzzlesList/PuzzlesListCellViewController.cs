using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;

namespace PixPuzzle
{
	[Register("PuzzlesListCellViewController")]
	public class PuzzlesListCellViewController : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("PuzzlesListViewCell");
		public static readonly UINib Nib = Nib = UINib.FromName ("PuzzlesListViewCell", NSBundle.MainBundle);

		public PuzzlesListCellViewController (IntPtr handle) : base (handle)
		{
			ExclusiveTouch = true;
			MultipleTouchEnabled = false;
		}

		public void UpdatePuzzleView (PuzzleData puzzle)
		{
			// Code specific view properties
			this.Layer.BorderWidth = 1.0f;
			this.Layer.BorderColor = UIColor.Black.CGColor;

			if (this.Selected) {
				SetSelected ();
			} else {
				UnsetSelected ();
			}
		}

		public void SetSelected ()
		{
			this.BackgroundColor = UIColor.Cyan;
		}

		public void UnsetSelected ()
		{
			this.BackgroundColor = UIColor.White;
		}

		public static PuzzlesListCellViewController Create (PuzzleData puzzle)
		{
			PuzzlesListCellViewController cell = Nib.Instantiate (null, null) [0] as PuzzlesListCellViewController;

			cell.UpdatePuzzleView (puzzle);

			// Get components. Check XCode for tags.
			// 1 = image
			// 2 = friends icon
			// 3 = completed icon
			UIImageView imageView = cell.ViewWithTag (1) as UIImageView;
			imageView.Image = UIImage.FromFile (puzzle.Filename);

			UIImageView friendsIcon = cell.ViewWithTag (2) as UIImageView;
			friendsIcon.Hidden = !(puzzle.IsCustom);

			UIImageView completedIcon = cell.ViewWithTag (3) as UIImageView;
			completedIcon.Hidden = !(puzzle.BestScore.HasValue);

			return cell;
		}
	}
}

