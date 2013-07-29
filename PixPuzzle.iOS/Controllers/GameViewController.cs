using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using PixPuzzle.Data;
using System.Threading;

namespace PixPuzzle
{
	public partial class GameViewController : UIViewController
	{
		private IGrid mGrid;
		private UIView mGridUIView;
		private UIImage mSelectedPuzzle;
		private NSTimer mTimer;
		private DateTime mCurrentTime;
		private bool mIsPaused;

		public GameViewController (IntPtr handle)
			: base(handle)
		{
		}

		public void DefinePuzzle (PuzzleData puzzle, UIImage selectedPuzzle)
		{
			this.mSelectedPuzzle = selectedPuzzle;

			// Load the image
			UIImage image = selectedPuzzle;
			Bitmap bitmap = new Bitmap (image);

			mGridUIView = initializeGrid (puzzle, image, bitmap);

			mIsPaused = false;
			mCurrentTime = new DateTime (0);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBarHidden = true;
			NavigationController.NavigationBar.Hidden = true;

			ScrollViewGame.ViewForZoomingInScrollView = new UIScrollViewGetZoomView ((sv) => {
				return mGridUIView;
			});

			// Margin
			int margin = mGrid.CellSize * 4;

			// Center the grid
			ScrollViewGame.ContentSize = new SizeF (mGridUIView.Frame.Width + margin, mGridUIView.Frame.Height + margin);

			PointF center = new PointF (ScrollViewGame.ContentSize.Width / 2, ScrollViewGame.ContentSize.Height / 2);
			ScrollViewGame.ContentOffset = new PointF (center.X / 2, center.Y / 2);

			// Scrolling with two fingers
			foreach (UIGestureRecognizer gestureRecognizer in ScrollViewGame.GestureRecognizers) {     
				if (gestureRecognizer is UIPanGestureRecognizer) {
					UIPanGestureRecognizer panGR = (UIPanGestureRecognizer)gestureRecognizer;
					panGR.MinimumNumberOfTouches = 2;               
					panGR.MaximumNumberOfTouches = 2;
				}
			}

			mGridUIView.Center = center;
			ScrollViewGame.AddSubview (mGridUIView);

			// Set timer in a thread
			var thread = new Thread (initializeTimer as ThreadStart);
			thread.Start ();
		}

		private void initializeTimer ()
		{
			const float updateTimerFrequency = 1f;

			using (var pool = new NSAutoreleasePool()) {

				// Every 1 sec we update game timer
				mTimer = NSTimer.CreateRepeatingScheduledTimer (updateTimerFrequency, delegate { 

					if (mIsPaused == false) {
						mCurrentTime = mCurrentTime.AddSeconds (updateTimerFrequency);

						this.InvokeOnMainThread (() => {
							LabelTime.Text = mCurrentTime.ToString ("mm:ss");
						});
					}
				});

				NSRunLoop.Current.Run ();
			}
		}

		private void stopTimer ()
		{
			if (mTimer != null) {
				mTimer.Dispose ();
				mTimer = null;
			}
		}

		/// <summary>
		/// Create a UIView containing the game
		/// </summary>
		/// <returns>The grid.</returns>
		/// <param name="puzzle">Puzzle.</param>
		/// <param name="image">Image.</param>
		/// <param name="bitmap">Bitmap.</param>
		private UIView initializeGrid (PuzzleData puzzle, UIImage image, Bitmap bitmap)
		{
			this.mGrid = null;
			UIView view = null;

			var pathGrid = new PathGridView (puzzle, (int)image.Size.Width, (int)image.Size.Height);
			view = pathGrid.GridViewInternal;

			mGrid = pathGrid;

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

			this.mGrid.GridCompleted += gridCompleted;
			this.mGrid.SetupGrid (pixels);

			return view;
		}

		private void gridCompleted ()
		{
			stopTimer ();

			UIAlertView alert = new UIAlertView (
				"Game Over",
				"You did it! " + mSelectedPuzzle,
				null,
				"OK");

			alert.Dismissed += (object sender, UIButtonEventArgs e) => {
				GoBackToMenu ();
			};
			alert.Show ();
		}

		partial void OnButtonQuitPressed (MonoTouch.Foundation.NSObject sender)
		{
			stopTimer ();
			GoBackToMenu ();
		}

		partial void OnButtonPausePressed (MonoTouch.Foundation.NSObject sender)
		{
			mIsPaused = !mIsPaused;

			if(mIsPaused) 
			{
				mGridUIView.Alpha = 0.2f;
				LabelTime.Text = "Pause";
				ButtonPause.SetTitle("Resume", UIControlState.Normal);
			}
			else {
				mGridUIView.Alpha = 1f;
				LabelTime.Text = "Resuming";
				ButtonPause.SetTitle("Pause", UIControlState.Normal);
			}
		}

		private void GoBackToMenu ()
		{
			NavigationController.PopToRootViewController (true);

//			var vc = this.Storyboard.InstantiateViewController ("MenuViewController") as UIViewController;
//			NavigationController.PresentViewController(
//				vc,
//				true
//				);
		}

		/// <summary>
		/// We store the grid object even if we're not using it so it is not garbage collected by mistake
		/// </summary>
		/// <value>The grid.</value>
		public IGrid Grid {
			get {
				return mGrid;
			}
		}
	}
}

