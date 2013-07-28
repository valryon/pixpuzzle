using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;

namespace PixPuzzle
{
	[Register("FriendsPuzzleListCellViewController")]
	public class FriendsPuzzleListCellViewController : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("FriendsPuzzleListCell");
		public static readonly UINib Nib = Nib = UINib.FromName ("FriendsPuzzleListCell", NSBundle.MainBundle);

		public FriendsPuzzleListCellViewController (IntPtr handle) : base (handle)
		{
		}

		public static FriendsPuzzleListCellViewController Create (PuzzleData puzzle)
		{
			FriendsPuzzleListCellViewController cell = Nib.Instantiate (null, null) [0] as FriendsPuzzleListCellViewController;

			return cell;
		}
	}
}

