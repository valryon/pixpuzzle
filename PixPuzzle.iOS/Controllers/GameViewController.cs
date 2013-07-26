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
		private IGrid grid;
		private UIView gridUIView;
		private UIImage selectedPuzzle;

		public GameViewController(IntPtr handle)
			: base(handle)
		{
		}

		public GameViewController (PuzzleData puzzle, UIImage selectedPuzzle)
			: base (null, null)
		{
			this.selectedPuzzle = selectedPuzzle;

			// Load the image
			UIImage image = selectedPuzzle;
			Bitmap bitmap = new Bitmap (image);

			gridUIView = initializeGrid (puzzle, image, bitmap);
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

			View.BackgroundColor = UIColor.FromPatternImage (new UIImage("background.png"));

			// Setup scrollview
			UIScrollView scrollView = new UIScrollView (new RectangleF(0,0,UIScreen.MainScreen.Bounds.Height,UIScreen.MainScreen.Bounds.Width));
			scrollView.ScrollEnabled = true;
			scrollView.MinimumZoomScale = 0.5f;
			scrollView.MaximumZoomScale = 2f;
			scrollView.BouncesZoom = true;

			scrollView.ViewForZoomingInScrollView = new UIScrollViewGetZoomView((sv) => {
				return gridUIView;
			});

			// Margin
			int margin = grid.CellSize * 4;

			// Center the grid
			scrollView.ContentSize = new SizeF (gridUIView.Frame.Width + margin, gridUIView.Frame.Height + margin);

//			PointF center = new PointF(scrollView.ContentSize.Width/2, scrollView.ContentSize.Height/2);
//			scrollView.ContentOffset = new PointF (center.X/2, center.Y/2);

			// Scrolling with two fingers
			foreach (UIGestureRecognizer gestureRecognizer in scrollView.GestureRecognizers) {     
				if (gestureRecognizer is UIPanGestureRecognizer) {
					UIPanGestureRecognizer panGR = (UIPanGestureRecognizer)gestureRecognizer;
					panGR.MinimumNumberOfTouches = 2;               
					panGR.MaximumNumberOfTouches = 2;
				}

			}

//			gridUIView.Center = center;
			scrollView.AddSubview (gridUIView);
			View.AddSubview (scrollView);
		}

		private UIView initializeGrid (PuzzleData puzzle, UIImage image, Bitmap bitmap) {
			this.grid = null;
			UIView view = null;

			var pathGrid = new PathGridView (puzzle, (int)image.Size.Width, (int)image.Size.Height);
			view = pathGrid.GridViewInternal;

			grid = pathGrid;

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
				"You did it! " + selectedPuzzle,
				null,
				"OK");

			alert.Dismissed += (object sender, UIButtonEventArgs e) => {
				var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 
//				appDelegate.ShowMenu ();
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

