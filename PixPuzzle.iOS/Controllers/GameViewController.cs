using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using PixPuzzle.Data;
using System.Threading;

namespace PixPuzzle
{
	/// <summary>
	/// The main view of the game, containing the complex PathGridView
	/// </summary>
	public partial class GameViewController : UIViewController
	{
		#region Fields

		private UIView mGridUIView;
		private PathGridView mPathGrid;
		private NSTimer mTimer;
		private DateTime mCurrentTime;
		private bool mIsPaused;

		#endregion

		#region Constructor and Initialization

		/// <summary>
		/// Called by the storyboard
		/// </summary>
		/// <param name="handle">Handle.</param>
		public GameViewController (IntPtr handle)
			: base(handle)
		{
		}

		/// <summary>
		/// Defines the puzzle the view should display
		/// </summary>
		/// <param name="puzzle">Puzzle.</param>
		/// <param name="selectedPuzzle">Selected puzzle.</param>
		public void DefinePuzzle (PuzzleData puzzle, UIImage selectedPuzzle)
		{
			this.Puzzle = puzzle;

			// Load the image
			UIImage image = selectedPuzzle;
			Bitmap bitmap = new Bitmap (image);

			mGridUIView = InitializeGrid (puzzle, image, bitmap);

			mIsPaused = false;
			mCurrentTime = new DateTime (0);
		}

		/// <summary>
		/// Create a UIView containing the game
		/// </summary>
		/// <returns>The grid.</returns>
		/// <param name="puzzle">Puzzle.</param>
		/// <param name="image">Image.</param>
		/// <param name="bitmap">Bitmap.</param>
		private UIView InitializeGrid (PuzzleData puzzle, UIImage image, Bitmap bitmap)
		{
			UIView view = null;

			// Here we create the real game view
			mPathGrid = new PathGridView (puzzle, (int)image.Size.Width, (int)image.Size.Height);
			mPathGrid.GridCompleted += GridCompleted;

			// Store it to prevent garbage collection
			view = mPathGrid.GridViewInternal;

			// Look at each pixel of the image
			CellColor[][] pixels = new CellColor[(int)image.Size.Width][];

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

			// Transfer pixel data to the internal grid
			this.mPathGrid.SetupGrid (pixels);

			return view;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBarHidden = true;
			NavigationController.NavigationBar.Hidden = true;

			ScrollViewGame.ViewForZoomingInScrollView = new UIScrollViewGetZoomView ((sv) => {
				return mGridUIView;
			});

			ScrollViewGame.DidZoom += (object sender, EventArgs e) => {

				// Always center content is the image is not entirely visible
				UIView subView = ScrollViewGame.Subviews[0];

				double offsetX = (ScrollViewGame.Bounds.Width > ScrollViewGame.ContentSize.Width)? 
					(ScrollViewGame.Bounds.Width - ScrollViewGame.ContentSize.Width) * 0.5 : 0.0;

				double offsetY = (ScrollViewGame.Bounds.Height > ScrollViewGame.ContentSize.Height)? 
					(ScrollViewGame.Bounds.Height - ScrollViewGame.ContentSize.Height) * 0.5 : 0.0;

				subView.Center = new PointF((float)(ScrollViewGame.ContentSize.Width  * 0.5 + offsetX), 
				                            (float)(ScrollViewGame.ContentSize.Height * 0.5 + offsetY));

				// Change the way cells are displayed
				const float zoomLimit = 0.45f;

				if (mPathGrid.ShouldDisplayFilledCells == false) {
					if (ScrollViewGame.ZoomScale <= zoomLimit) {
						mPathGrid.ShouldDisplayFilledCells = true;

						RectangleF visibleRect = ScrollViewGame.ConvertRectToView (ScrollViewGame.Bounds, mGridUIView);
						mGridUIView.SetNeedsDisplayInRect (visibleRect);
					}
				} else {
					if (ScrollViewGame.ZoomScale > zoomLimit) {
						mPathGrid.ShouldDisplayFilledCells = false;

						RectangleF visibleRect = ScrollViewGame.ConvertRectToView (ScrollViewGame.Bounds, mGridUIView);
						mGridUIView.SetNeedsDisplayInRect (visibleRect);
					}
				}

			};

			// Margin
//			int margin = 21;

			// Center the grid
//			ScrollViewGame.ContentSize = new SizeF (mGridUIView.Frame.Width + margin, mGridUIView.Frame.Height + margin);

//			PointF center = new PointF (ScrollViewGame.ContentSize.Width / 2, ScrollViewGame.ContentSize.Height / 2);
//			ScrollViewGame.ContentOffset = new PointF (center.X / 2, center.Y / 2);

			// Scrolling with two fingers
			foreach (UIGestureRecognizer gestureRecognizer in ScrollViewGame.GestureRecognizers) {     
				if (gestureRecognizer is UIPanGestureRecognizer) {
					UIPanGestureRecognizer panGR = (UIPanGestureRecognizer)gestureRecognizer;
					panGR.MinimumNumberOfTouches = 2;               
					panGR.MaximumNumberOfTouches = 2;
				}
			}

//			mGridUIView.Center = center;
			ScrollViewGame.AddSubview (mGridUIView);

			// Set timer in a thread
			var thread = new Thread (InitializeTimer as ThreadStart);
			thread.Start ();
		}

		private void InitializeTimer ()
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

		#endregion

		#region Methods

		private void StopTimer ()
		{
			if (mTimer != null) {
				mTimer.Dispose ();
				mTimer = null;
			}
		}

		private void GridCompleted ()
		{
			StopTimer ();

			// Register the score
			this.Puzzle.AddPlayerScore (GameCenterHelper.LocalPlayer.PlayerID, true, mCurrentTime);
			PuzzleService.Instance.Save ();

			// Is it a versus match?
			if (Puzzle.Match != null) {
				// If so, send the score to the friend
				GameCenterHelper.UpdateMatchFromPuzzle (Puzzle, (err) => {
					UIAlertView alert = new UIAlertView (
						"Versus",
						"Data sent? " + err,
						null,
						"TODO");

					alert.Dismissed += (object sender, UIButtonEventArgs e) => {
						GoBackToMenu ();
					};
					alert.Show ();
				});
			} else {
				UIAlertView alert = new UIAlertView (
					"Game Over",
					"You did it! ",
					null,
					"OK");

				alert.Dismissed += (object sender, UIButtonEventArgs e) => {
					GoBackToMenu ();
				};
				alert.Show ();
			}
		}

		private void GoBackToMenu ()
		{
			NavigationController.PopToRootViewController (false);
//
//			var vc = this.Storyboard.InstantiateViewController ("MenuPlayViewController") as UIViewController;
//			NavigationController.PushViewController(
//				vc,
//				true
//				);
		}

		#endregion

		#region Events

		partial void OnButtonQuitPressed (MonoTouch.Foundation.NSObject sender)
		{
			StopTimer ();
			GoBackToMenu ();
		}

		partial void OnButtonPausePressed (MonoTouch.Foundation.NSObject sender)
		{
			mIsPaused = !mIsPaused;

			if (mIsPaused) {
				mGridUIView.Alpha = 0.2f;
				LabelTime.Text = "Pause";
				ButtonPause.SetTitle ("Resume", UIControlState.Normal);
			} else {
				mGridUIView.Alpha = 1f;
				LabelTime.Text = "Resuming";
				ButtonPause.SetTitle ("Pause", UIControlState.Normal);
			}
		}

		partial void OnButtonDebugPressed (MonoTouch.Foundation.NSObject sender)
		{
			GridCompleted ();
		}

		#endregion

		#region Properties

		public PuzzleData Puzzle {
			get;
			private set;
		}

		#endregion
	}
}

