using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.GameKit;
using PixPuzzle.Data;
using System.Threading;

namespace PixPuzzle
{
	public partial class MenuCreateStep2ViewController : UIViewController
	{
		private const int NORMAL_SIZE = 64;
		private const int HARD_SIZE = 96;
		private const int EXPERT_SIZE = 128;
		private UIImage mCleanImage, mImageToUse;
		private bool mIsFriendMatch;
		private Thread mFilterThread;
		private int mLastDifficulty;

		public MenuCreateStep2ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Display the image
			ImageTransformed.Image = mImageToUse;

			// Setup difficulty slider
			SliderDifficulty.SetValue (1, false);
			SliderDifficulty.Continuous = true;
			SliderDifficulty.ValueChanged += (object sender, EventArgs e) => {
				SliderDifficulty.SetValue ((float)Math.Round (SliderDifficulty.Value), false);
			};
			SliderDifficulty.TouchUpInside += (object sender, EventArgs e) => {
				// Slider released: update img
				FilterImage (mCleanImage, (int)SliderDifficulty.Value);
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.mIsFriendMatch) {
				ButtonShare.Hidden = true;
			} else {
				ButtonShare.Hidden = false;
			}
		}

		public void InitializePuzzleCreation (UIImage cleanImage, bool isFriendMatch)
		{
			this.mCleanImage = cleanImage;
			this.mIsFriendMatch = isFriendMatch;

			mLastDifficulty = 0;
			FilterImage (cleanImage, 1);
		}

		public void FilterImage (UIImage img, int difficulty)
		{
			if (mLastDifficulty != difficulty) {

				mLastDifficulty = difficulty;

				if (mFilterThread != null) {
					mFilterThread.Abort ();
					mFilterThread = null;
				}

				mFilterThread = new Thread (() => {
					this.mImageToUse = img;

					// Slight saturation
					mImageToUse = UIImageEx.AdjustBrightnessSaturationAndContrast (mImageToUse, 0, 1.2f);

					int size = HARD_SIZE;
					if (difficulty <= 0) {
						size = NORMAL_SIZE;
					} else if (difficulty > 0 && difficulty <= 1) {
						size = HARD_SIZE;
					} else if (difficulty > 1) {
						size = EXPERT_SIZE;
					}

					mImageToUse = ImageFilters.Filter (mImageToUse, size);

					InvokeOnMainThread (() => {
						if (ImageTransformed != null) {
							ImageTransformed.Image = mImageToUse;
						}
					});
				});
				mFilterThread.Start ();
			}
		}

		partial void OnPlayButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			if (mIsFriendMatch) {
				SharePuzzle ();
			} else {
				LaunchPuzzleForCurrentImage (null);
			}
		}

		partial void OnShareButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			SharePuzzle ();
		}

		void SharePuzzle ()
		{
			// Send to a friend
			GameCenterHelper.NewVersusPhoto (ui => {
				InvokeOnMainThread (() => {
					PresentViewController (ui, true, null);
				});
			}, matchPuzzle => {
				// Launch level
				LaunchPuzzleForCurrentImage (matchPuzzle);
			}, null, null, null);
		}

		void LaunchPuzzleForCurrentImage (PuzzleData matchPuzzle)
		{
			// Register a new puzzle
			string me = (GKLocalPlayer.LocalPlayer.Authenticated ? GKLocalPlayer.LocalPlayer.PlayerID : "Me");
			PuzzleData puzzle = PuzzleService.Instance.AddPuzzle (Guid.NewGuid () + ".png", me, mImageToUse);

			if (matchPuzzle != null) {
				puzzle.Match = matchPuzzle.Match;
			}

			// Prepare game
			var vc = this.Storyboard.InstantiateViewController ("GameViewController") as GameViewController;
			vc.DefinePuzzle (puzzle, mImageToUse);

			NavigationController.PushViewController (vc, true);
		}
	}
}

