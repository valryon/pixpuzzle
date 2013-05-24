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
		private IGrid grid;
		private UIView gridUIView;
		private GameModes mode;
		private string selectedPuzzleFile;

		public GameViewController (GameModes mode, string selectedPuzzleFile)
			: base (null, null)
		{
			this.mode = mode;
			this.selectedPuzzleFile = selectedPuzzleFile;

			// Load the image
			UIImage image = UIImage.FromFile (selectedPuzzleFile);
			Bitmap bitmap = new Bitmap (image);

			gridUIView = initializeGrid (image, bitmap, mode);
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

			// Setup scrollview
			UIScrollView scrollView = new UIScrollView (new RectangleF(0,0,UIScreen.MainScreen.Bounds.Height,UIScreen.MainScreen.Bounds.Width));
			scrollView.ScrollEnabled = true;
			scrollView.BackgroundColor = UIColor.Gray;

			// Margin
			float contentX = 32;
			float contentY = 32;
			scrollView.ContentSize = new SizeF (gridUIView.Frame.Width + (contentX*2), gridUIView.Frame.Height + (contentY * 2));

			// Scrolling with two fingers
			foreach (UIGestureRecognizer gestureRecognizer in scrollView.GestureRecognizers) {     
				if (gestureRecognizer is UIPanGestureRecognizer) {
					UIPanGestureRecognizer panGR = (UIPanGestureRecognizer)gestureRecognizer;
					panGR.MinimumNumberOfTouches = 2;               
					panGR.MaximumNumberOfTouches = 2;
				}

			}

			scrollView.AddSubview (gridUIView);
			View.AddSubview (scrollView);
		}

		private UIView initializeGrid (UIImage image, Bitmap bitmap, GameModes mode) {
			this.grid = null;
			UIView view = null;

			if(mode == GameModes.Path) {
				var pathGrid = new PathGridView ((int)image.Size.Width, (int)image.Size.Height);
				view = pathGrid.GridViewInternal;

				grid = pathGrid;
			}
			else {
				var picrossGrid = new PicrossGridView ((int)image.Size.Width, (int)image.Size.Height);
				view = picrossGrid.PicrossGridViewInternal;

				grid = picrossGrid;
			}

			CellColor[][] pixels = new CellColor[(int)image.Size.Width][];

			// Look at each pixel
			for (int x=0; x<image.Size.Width; x++) {

				pixels [x] = new CellColor[(int)image.Size.Height];

				for (int y=0; y<image.Size.Height; y++) {

					// Get the pixel color
					Color c = bitmap.GetPixel (x, y);

					// Transform to generic color
					pixels [x] [y] = new CellColor () {
						A = c.A/255f,
						R = c.R/255f, 
						G = c.G/255f, 
						B = c.B/255f
					};

				}
			}

			this.grid.GridCompleted += gridCompleted;
			this.grid.SetupGrid (pixels);

			return view;
		}

		private void gridCompleted ()
		{
			UIAlertView alert = new UIAlertView (
				"Game Over",
				"You did it! " + selectedPuzzleFile + " " + mode,
				null,
				"OK");

			alert.Dismissed += (object sender, UIButtonEventArgs e) => {
				var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
				appDelegate.ShowMenu ();
			};
			alert.Show ();
		}
		/// <summary>
		/// We store the grid object even if we're not using it so it is not garbage collected by mistake
		/// </summary>
		/// <value>The grid.</value>
		public IGrid Grid {
			get {
				return grid;
			}
		}
	}
}

