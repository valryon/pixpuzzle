using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PixPuzzle
{
	public partial class FriendsPuzzleListNewCellViewController : UICollectionViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("FriendsPuzzleListNewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("FriendsPuzzleListNewCell");

		public FriendsPuzzleListNewCellViewController (IntPtr handle) : base (handle)
		{
		}

		public static FriendsPuzzleListNewCellViewController Create ()
		{
			return (FriendsPuzzleListNewCellViewController)Nib.Instantiate (null, null) [0];
		}
	}
}

