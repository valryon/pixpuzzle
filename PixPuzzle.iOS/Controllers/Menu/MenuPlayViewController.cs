using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;

namespace PixPuzzle
{
	public partial class MenuPlayViewController : UIViewController
	{
		private PuzzleData lastSelectedPuzzle;

		public MenuPlayViewController (IntPtr handle) : base (handle)
		{
			lastSelectedPuzzle = null;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		
			// Select last puzzle not completed
			var puzzles = PuzzleService.Instance.GetPuzzles ();
			PuzzleData selectedPuzzle = puzzles.Where(p => p.Scores.Any() == false).FirstOrDefault();

			if (selectedPuzzle == null) {
				selectedPuzzle = puzzles.Last ();
			}

			SetSelectedPuzzle (selectedPuzzle);

			// Select in collection...
			var listView = this.ChildViewControllers
				.First (vc => vc is PuzzleListViewController) as PuzzleListViewController;

			listView.SelectPuzzleManually (selectedPuzzle);
		}

		public void SetSelectedPuzzle (PuzzleData puzzle)
		{
			lastSelectedPuzzle = puzzle;

			var detailView = this.ChildViewControllers
				.First (vc => vc is PuzzleListDetailViewController) as PuzzleListDetailViewController;

			if (puzzle == null) {
				detailView.View.Hidden = true;
			} else {
				detailView.View.Hidden = false;

				detailView.SetPuzzle (puzzle);
			}
		}

		public void PlaySelectedPuzzle() 
		{
			if (lastSelectedPuzzle != null) {
				var vc = this.Storyboard.InstantiateViewController ("GameViewController") as GameViewController;

				vc.DefinePuzzle (lastSelectedPuzzle, UIImage.FromFile (lastSelectedPuzzle.Filename));

				NavigationController.PushViewController (
					vc,
					true
				);
			}
		}
	}
}

