using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;

namespace PixPuzzle
{
	public partial class PuzzleListDetailViewController : UIViewController
	{
		public PuzzleListDetailViewController (IntPtr handle) : base (handle)
		{
		}

		/// <summary>
		/// Update the detail view with a puzzle
		/// </summary>
		/// <param name="puzzle">Puzzle.</param>
		public void SetPuzzle (PuzzleData puzzle)
		{
			ImagePuzzle.Image = UIImage.FromFile (puzzle.Filename);
			LabelTitle.Text = System.IO.Path.GetFileNameWithoutExtension (puzzle.Filename);

			if (puzzle.BestScore.HasValue) {
				LabelTime.Text = puzzle.BestScore.Value.ToString("mm min ss sec");
			} else {
				LabelTime.Text = "Not completed yet";
			}
		}

		partial void OnButtonPlayPressed (MonoTouch.Foundation.NSObject sender)
		{
			MenuPlayViewController playVc = ParentViewController as MenuPlayViewController;
			playVc.PlaySelectedPuzzle();
		}
	}
}

