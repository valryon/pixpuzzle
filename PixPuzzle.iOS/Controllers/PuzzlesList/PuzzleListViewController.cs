using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;
using System.Collections.Generic;

namespace PixPuzzle
{
	[Register ("PuzzleListViewController")]
	public class PuzzleListViewController : UICollectionViewController
	{
		private List<PuzzleData> puzzlesPxn, puzzlesCustom;
		private PuzzlesListCellViewController lastSelectedCell;

		public PuzzleListViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		private void Initialize ()
		{
			var allPuzzles = PuzzleService.Instance.GetPuzzles ();

			puzzlesPxn = allPuzzles.Where (p => p.IsCustom == false).ToList ();
			puzzlesCustom = allPuzzles.Where (p => p.IsCustom == true).ToList ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CollectionView.RegisterClassForCell (typeof(PuzzlesListCellViewController), PuzzlesListCellViewController.Key);
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
			PuzzlesListCellViewController cell = PuzzlesListCellViewController.Create (puzzle);

			return cell;
		}
	
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			lastSelectedCell.UnsetSelected ();

			var cell = collectionView.CellForItem (indexPath) as PuzzlesListCellViewController;
			lastSelectedCell = cell;
			cell.SetSelected ();

			var puzzle = GetPuzzleForPath (indexPath);

			MenuPlayViewController menu = ParentViewController as MenuPlayViewController;
			menu.SetSelectedPuzzle (puzzle);
		}

		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath) as PuzzlesListCellViewController;

			cell.UnsetSelected ();

			MenuPlayViewController menu = ParentViewController as MenuPlayViewController;
			menu.SetSelectedPuzzle (null);
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

		internal void SelectPuzzleManually (PuzzleData selectedPuzzle)
		{
			int index = -1;
			int section = -1;

			if (puzzlesPxn.Contains (selectedPuzzle)) {
				section = 0;
				index = puzzlesPxn.IndexOf (selectedPuzzle);
			} else if (puzzlesCustom.Contains (selectedPuzzle)) {
				section = 1;
				index = puzzlesCustom.IndexOf (selectedPuzzle);
			}

			NSIndexPath path = NSIndexPath.FromItemSection (index, section);
			var cell = CollectionView.CellForItem (path) as PuzzlesListCellViewController;
			cell.SetSelected ();

			lastSelectedCell = cell;
		}
	}
}

