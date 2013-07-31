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
		private UICollectionViewCell lastSelectedCell;

		public FriendsPuzzleListViewController (IntPtr handle) : base (handle)
		{
			puzzlesWitHFriends = new List<PuzzleData> ();
			LoadMatches ();
		}

		public void LoadMatches ()
		{
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
			if (lastSelectedCell != null) {
				lastSelectedCell.BackgroundColor = UIColor.Clear;
			}

			var cell = collectionView.CellForItem (indexPath);
			cell.BackgroundColor = UIColor.Cyan;

			lastSelectedCell = cell;

			if (cell is FriendsPuzzleListCellViewController) {
				var friendsCell = cell as FriendsPuzzleListCellViewController;
				var puzzle = puzzlesWitHFriends [indexPath.Item - 1];


			} else if (cell is FriendsPuzzleListNewCellViewController) {

				var vc = this.Storyboard.InstantiateViewController("MenuCreateViewController") as MenuCreateViewController;
				vc.IsFriendMatch = true;

				NavigationController.PushViewController(
					vc,
					true
					);
			}
		}

		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath);
			cell.BackgroundColor = UIColor.Clear;

			if (cell is FriendsPuzzleListCellViewController) {
//				var friendsCell = cell as FriendsPuzzleListCellViewController;
//				var puzzle = puzzlesWitHFriends [indexPath.Item - 1];
			}
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
			} else {
//				var cell = collectionView.DequeueReusableCell (FriendsPuzzleListCellViewController.Key, indexPath) as FriendsPuzzleListCellViewController;

				// Get puzzle
				// Index - 1 because the first cell if for the adding thing
				var puzzle = puzzlesWitHFriends [indexPath.Item - 1];

				// Create cell
				var cell = FriendsPuzzleListCellViewController.Create (puzzle);
			
				return cell;
			}
		}
	}
}

