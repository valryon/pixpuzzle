using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.GameKit;
using PixPuzzle.Data;

namespace PixPuzzle
{
	public partial class MenuCreateStep2ViewController : UIViewController
	{
		private UIImage baseImage;
		private bool isFriendMatch;

		public MenuCreateStep2ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.isFriendMatch) {
				ButtonShare.Hidden = true;
			} else {
				ButtonShare.Hidden = false;
			}
		}

		public void SetBaseImage(UIImage img, bool isFriendMatch) 
		{
			this.baseImage = img;
			this.isFriendMatch = isFriendMatch;

			// Slight saturation
			baseImage = UIImageEx.AdjustBrightnessSaturationAndContrast (baseImage, 0, 1.2f);

			// 64 is already a BIG value
			baseImage = ImageFilters.Filter (baseImage, 96);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Display the image
			ImageTransformed.Image = baseImage;
		}

		partial void OnPlayButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			LaunchPuzzleForCurrentImage (null);
		}

		partial void OnShareButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			// Send to a friend
			GameCenterHelper.NewVersusPhoto (
				(ui) => {
					InvokeOnMainThread (() => {
						PresentViewController (ui, true, null);
					});
				},
				(matchPuzzle) => {
					// Launch level
					LaunchPuzzleForCurrentImage (matchPuzzle);
				}
			, null, null, null);
		}

		void LaunchPuzzleForCurrentImage (PuzzleData matchPuzzle)
		{
			// Register a new puzzle
			string me = (GKLocalPlayer.LocalPlayer.Authenticated ? GKLocalPlayer.LocalPlayer.PlayerID : "Me");
			PuzzleData puzzle = PuzzleService.Instance.AddPuzzle(Guid.NewGuid()+".png", me, baseImage);

			if (matchPuzzle != null) {
				puzzle.Match = matchPuzzle.Match;
			}

			// Prepare game
			var vc = this.Storyboard.InstantiateViewController ("GameViewController") as GameViewController;
			vc.DefinePuzzle (puzzle, baseImage);

			NavigationController.PushViewController (vc, true);
		}
	}
}

