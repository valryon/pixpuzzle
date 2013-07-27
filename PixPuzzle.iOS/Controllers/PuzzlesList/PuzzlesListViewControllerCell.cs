using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;

namespace PixPuzzle
{
	[Register("PuzzlesListViewControllerCell")]
	public class PuzzlesListViewControllerCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("PuzzlesListViewCell");
		public static readonly UINib Nib= Nib = UINib.FromName ("PuzzlesListViewCell", NSBundle.MainBundle);

		public PuzzlesListViewControllerCell(IntPtr handle) : base (handle)
		{
		}

		public static PuzzlesListViewControllerCell Create(PuzzleData puzzle) {

			PuzzlesListViewControllerCell cell = Nib.Instantiate (null, null) [0] as PuzzlesListViewControllerCell;

			// Code specific view properties
			cell.Layer.BorderWidth = 1.0f;
			cell.Layer.BorderColor = UIColor.Black.CGColor;

			if (cell.Selected) {
				cell.BackgroundColor = UIColor.Blue;
			}


			// Get components. Check XCode for tags.
			// 1 = image
			// 2 = title label
			// 3 = size label
			UIImageView imageView = cell.ViewWithTag (1) as UIImageView;
			imageView.Image = UIImage.FromFile (puzzle.Filename);

			UILabel titleLabel = cell.ViewWithTag (2) as UILabel;
			titleLabel.Text = System.IO.Path.GetFileNameWithoutExtension (puzzle.Filename);

			UILabel sizeLabel = cell.ViewWithTag (3) as UILabel;
			sizeLabel.Text = "42 x 42";

			return cell;
		}
	}
}

