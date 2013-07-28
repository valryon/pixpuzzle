using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PixPuzzle
{
	[Register("FriendsPuzzleListViewController")]
	public class FriendsPuzzleListViewController : UICollectionViewController
	{
		public FriendsPuzzleListViewController (IntPtr handle) : base (handle)
		{
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			// TODO: return the actual number of items in the section
			return 1;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
//			var cell = collectionView.DequeueReusableCell (FriendsPuzzleListCellViewController.Key, indexPath) as FriendsPuzzleListCellViewController;
			var cell = FriendsPuzzleListCellViewController.Create (null);
			
			return cell;
		}
	}
}

