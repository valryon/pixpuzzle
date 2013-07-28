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

		public MenuCreateStep2ViewController (IntPtr handle) : base (handle)
		{
		}

		public void SetBaseImage(UIImage img) 
		{
			baseImage = img;

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
			var vc = this.Storyboard.InstantiateViewController("GameViewController") as GameViewController;

			// Register a new puzzle
			string me = (GKLocalPlayer.LocalPlayer.Authenticated ? GKLocalPlayer.LocalPlayer.PlayerID : "Me");
			PuzzleData puzzle = PuzzleService.Instance.AddPuzzle(Guid.NewGuid()+".png", me, baseImage);

			// Prepare game
			vc.DefinePuzzle(puzzle, baseImage);

			NavigationController.PushViewController(
				vc,
				true
				);
		}

		partial void OnShareButtonPressed (MonoTouch.Foundation.NSObject sender)
		{

		}
	}
}

