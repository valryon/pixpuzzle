using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using PixPuzzle.Data;

namespace PixPuzzle
{
	public partial class GameViewController : UIViewController
	{
		public static string ImageDirectory = "puzzles/";
		private GridView grid;
		private string selectedPuzzleFile;

		public GameViewController (string selectedPuzzleFile)
			: base (null, null)
		{
			this.selectedPuzzleFile = selectedPuzzleFile;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Landscape;
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
			
			// Load the image
			UIImage image = UIImage.FromFile (selectedPuzzleFile);
			Bitmap bitmap = new Bitmap (image);

			grid = new GridView ((int)image.Size.Width, (int)image.Size.Height);
			grid.GridCompleted += gridCompleted;

			// Setup scrollview
			UIScrollView scrollView = new UIScrollView (new RectangleF(0,0,UIScreen.MainScreen.Bounds.Height,UIScreen.MainScreen.Bounds.Width));
			scrollView.ScrollEnabled = true;
			scrollView.BackgroundColor = UIColor.Gray;

			// Margin
			float contentX = 32;
			float contentY = 32;
			scrollView.ContentSize = new SizeF (grid.View.Frame.Width + (contentX*2), grid.View.Frame.Height + (contentY*2));

			// Scrolling with two fingers
			foreach (UIGestureRecognizer gestureRecognizer in scrollView.GestureRecognizers) {     
				if (gestureRecognizer is UIPanGestureRecognizer) {
					UIPanGestureRecognizer panGR = (UIPanGestureRecognizer)gestureRecognizer;
					panGR.MinimumNumberOfTouches = 2;               
					panGR.MaximumNumberOfTouches = 2;
				}

			}

			scrollView.AddSubview (grid.View);
			View.AddSubview (scrollView);

			// Look at each pixel
			for (int x=0; x<image.Size.Width; x++) {
				for (int y=0; y<image.Size.Height; y++) {

					// Get the pixel color
					Color c = bitmap.GetPixel (x, y);

					// Cleaning images
					CellColor cellColor;
					if (c.A > 20) {
						cellColor = new CellColor () {
							A = /*c.A/255f*/ 1f,
							R = c.R/255f, 
							G = c.G/255f, 
							B = c.B/255f
						};
					} else {
						cellColor = new CellColor () {
							A = 1f,
							R = 1f, 
							G = 1f, 
							B = 1f // White
						};
					}

					grid.SetPixelData (x, y, cellColor);
				}	
			}

			// Launch the setup process
			grid.SetupGrid ();

			// Prepare the drawing, place the grid where it should be
			grid.View.InitializeViewForDrawing ((int)contentX, (int)contentY);
		}

		private void gridCompleted ()
		{
			UIAlertView alert = new UIAlertView (
				"Game Over",
				"You did it!",
				null,
				"OK");

			alert.Dismissed += (object sender, UIButtonEventArgs e) => {
				var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
				appDelegate.ShowMenu();
			};
			alert.Show ();
		}
	}
}

