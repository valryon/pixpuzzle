using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;
using System.Collections.Generic;

namespace PixPuzzle
{
	[Register ("MenuPlayViewController")]
	public class MenuPlayViewController : UICollectionViewController
	{
		private List<PuzzleData> puzzlesPxn, puzzlesCustom;

		public MenuPlayViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		private void Initialize ()
		{
			var allPuzzles = PuzzleService.Instance.GetPuzzles ();

			puzzlesPxn = allPuzzles.Where (p => p.IsCustom == false).ToList ();
			puzzlesCustom = allPuzzles.Where (p => p.IsCustom == true).ToList ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CollectionView.RegisterClassForCell (typeof(PuzzlesListViewControllerCell), PuzzlesListViewControllerCell.Key);
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			// We have two sections : 
			// 1/ Puzzles embed in the app
			int sectionCount = 1;

			// 2/ Puzzles created by the user
			if (puzzlesCustom.Any ()) {
				sectionCount = 2;
			}
			return sectionCount;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			if (section == 0) {
				return puzzlesPxn.Count;
			} else if (section == 1) {
				return puzzlesCustom.Count;
			}

			return 0;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Get the appropriate puzzle
			var puzzle = GetPuzzleForPath (indexPath);

			// Populate view from puzzle data
			PuzzlesListViewControllerCell cell = PuzzlesListViewControllerCell.Create (puzzle);

			return cell;
		}
	
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath) as PuzzlesListViewControllerCell;

			cell.SetSelected ();
		}

		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath) as PuzzlesListViewControllerCell;

			cell.UnsetSelected ();
		}

		internal PuzzleData GetPuzzleForPath (NSIndexPath indexPath)
		{
			PuzzleData puzzle = null;
			if (indexPath.Section == 0) {
				puzzle = puzzlesPxn [indexPath.Item];
			}
			else {
				puzzle = puzzlesCustom [indexPath.Item];
			}
			return puzzle;
		}
	}
}

