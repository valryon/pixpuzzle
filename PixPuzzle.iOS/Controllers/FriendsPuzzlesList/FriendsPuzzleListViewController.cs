using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using PixPuzzle.Data;

namespace PixPuzzle
{
	[Register("FriendsPuzzleListViewController")]
	public class FriendsPuzzleListViewController : UICollectionViewController
	{
		private List<PuzzleData> puzzlesWitHFriends;

		public FriendsPuzzleListViewController (IntPtr handle) : base (handle)
		{
			puzzlesWitHFriends = new List<PuzzleData> ();
			LoadMatches ();
		}

		public void LoadMatches() {
			if (puzzlesWitHFriends != null) {
				puzzlesWitHFriends.Clear ();
			}

			if (GameCenterHelper.IsAuthenticated) {
				GameCenterHelper.GetFriendsPuzzles ((puzzles) => {
					puzzlesWitHFriends = puzzles;

					CollectionView.ReloadData ();
				});
			} else {
				// TODO Say that it requires GameCenter
				Logger.W ("Can't play with friends without Game Center");
			}
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			base.ItemSelected (collectionView, indexPath);
		}

		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			base.ItemDeselected (collectionView, indexPath);
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return puzzlesWitHFriends.Count + 1;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			if (indexPath.Item == 0) {
				// First cell is the adding item
				return FriendsPuzzleListNewCellViewController.Create ();
			} 
			else {
//			var cell = collectionView.DequeueReusableCell (FriendsPuzzleListCellViewController.Key, indexPath) as FriendsPuzzleListCellViewController;

				// Get puzzle
				var puzzle = puzzlesWitHFriends [indexPath.Item - 1];

				// Create cell
				var cell = FriendsPuzzleListCellViewController.Create (puzzle);
			
				return cell;
			}
		}
	}
}

