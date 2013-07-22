using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using PixPuzzle.Data;
using GPUImage.Filters;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;

namespace PixPuzzle
{
	public partial class MenuViewController : UIViewController
	{
		private UIView buttonPanel, levelSelectionPanel;

		public MenuViewController () : base (null, null)
		{
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Landscape;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			createView ();
			showButtonPanel ();

		}

		private void createView ()
		{
			View = new UIView (new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width));
			View.BackgroundColor = UIColor.FromPatternImage (new UIImage ("background.png"));

			// First buttons
			// --------------------------------
			buttonPanel = new UIView (new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width));

			int buttonWidth = 300;
			int buttonHeight = 80;
			RectangleF buttonRect = new RectangleF (UIScreen.MainScreen.Bounds.Height / 2 - buttonWidth / 2,
			                                        UIScreen.MainScreen.Bounds.Width / 2 - buttonHeight / 2,
			                                        buttonWidth,
			                                        buttonHeight
			);

			// Play button
			buttonRect.Y = UIScreen.MainScreen.Bounds.Width / 2 - 2 * buttonHeight;

			UIButton buttonPlay = new UIButton (buttonRect);
			buttonPlay.BackgroundColor = UIColor.White;
			buttonPlay.SetTitleColor (UIColor.Black, UIControlState.Normal);
			buttonPlay.Layer.BorderWidth = 2f;
			buttonPlay.Layer.BorderColor = UIColor.Blue.CGColor;
			buttonPlay.SetTitle ("Play!", UIControlState.Normal);
			buttonPlay.TouchUpInside += (object sender, EventArgs e) => {
				showLevelSelection ();
			};

			buttonPanel.AddSubview (buttonPlay);

			// Credits button
			buttonRect.Y = UIScreen.MainScreen.Bounds.Width / 2 + 2 * buttonHeight;

			UIButton buttonCredits = new UIButton (buttonRect);
			buttonCredits.BackgroundColor = UIColor.White;
			buttonCredits.SetTitleColor (UIColor.Black, UIControlState.Normal);
			buttonCredits.Layer.BorderWidth = 2f;
			buttonCredits.Layer.BorderColor = UIColor.Blue.CGColor;
			buttonCredits.SetTitle ("Credits", UIControlState.Normal);

			buttonPanel.AddSubview (buttonCredits);


			// Level selection
			// --------------------------------
			levelSelectionPanel = new UIView (new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width));
			UIScrollView scroll = new UIScrollView (levelSelectionPanel.Frame);

			levelSelectionPanel.AddSubview (scroll);

			int baseX = 40;
			int baseY = 40;
			int x = baseX;
			int y = baseY;
			int width = 120;
			int height = 140;
			RectangleF scrolLContentSize = new RectangleF ();
			scrolLContentSize.Width = 3 * x + 2 * width;

			int elementByLine = 6;
			int elementOnCurrentLine = 0;

			foreach (var puzzle in PuzzleService.Instance.GetPuzzles(false, false)) {
				UIButton levelButton = CreateLevelButton (x, y, width, height, puzzle);

				x += baseX + width;

				elementOnCurrentLine++;
				if (elementOnCurrentLine >= elementByLine) {
					elementOnCurrentLine = 0;

					x = baseX;

					int yJump = baseY + height;
					y += yJump;

					scrolLContentSize.Height += yJump;
				}

				scroll.AddSubview (levelButton);
			}

			UIButton pictureButton = new UIButton (new RectangleF (View.Frame.Width / 2 - width, 2 * y, width * 2, height / 2));
			pictureButton.BackgroundColor = UIColor.White;
			pictureButton.Layer.BorderColor = UIColor.Green.CGColor;
			pictureButton.Layer.BorderWidth = 3f;
			pictureButton.SetTitleColor (UIColor.Black, UIControlState.Normal);
			pictureButton.SetTitle ("Custom photo", UIControlState.Normal);
			pictureButton.TouchUpInside += (object sender, EventArgs e) => {

//				Camera.TakePicture (this, (dico) => {
//				Camera.SelectPicture(this, (dico) => {

				UIImage selectedImage = null;

				// Get camera result
//					var selectedImageObject = dico.ObjectForKey (UIImagePickerController.OriginalImage);
//
//					if (selectedImageObject != null && selectedImageObject is UIImage) {
//						selectedImage = selectedImageObject as UIImage;
//					}

				var img = createPuzzleFromPhoto (selectedImage);

				// DIsplay image and a way to play
				foreach (var v in View.Subviews) {
					v.RemoveFromSuperview ();
				}

				UIImageView imageView = new UIImageView (img);
				UIScrollView scrollView = new UIScrollView (View.Frame);
				scrollView.BackgroundColor = UIColor.LightGray;
				scrollView.Layer.BorderColor = UIColor.Red.CGColor;
				scrollView.Layer.BorderWidth = 4f;
				scrollView.ScrollEnabled = true;
				scrollView.MinimumZoomScale = 0.5f;
				scrollView.MaximumZoomScale = 2f;
				scrollView.BouncesZoom = true;

				scrollView.ViewForZoomingInScrollView = new UIScrollViewGetZoomView ((sv) => {
					return imageView;
				});
				scrollView.AddSubview (imageView);

				View.AddSubview (scrollView);

				// Display image
				UIButton playButton = new UIButton (new RectangleF (550, 250, 200, 100));
				playButton.SetTitle ("PLAY!", UIControlState.Normal);
				playButton.ContentMode = UIViewContentMode.ScaleToFill;
				playButton.BackgroundColor = UIColor.Black;
				playButton.Layer.BorderColor = UIColor.Red.CGColor;
				playButton.Layer.BorderWidth = 4f;

				View.AddSubview (playButton);

				playButton.TouchDown += (object s2, EventArgs e2) => {
					// Save level
					PuzzleData puzzle = PuzzleService.Instance.AddPuzzle("TODO","TODO",null);

					// Launch level
					launchLevel (puzzle, img);
				};
//				});
			};

			scroll.AddSubview (pictureButton);

			scrolLContentSize.Height += baseY;
			scroll.ContentSize = new SizeF (scrolLContentSize.Width, scrolLContentSize.Height);
		}

		static UIImage createPuzzleFromPhoto (UIImage selectedImage)
		{
			UIImage img;

			if (selectedImage == null) {
				img = UIImage.FromFile ("testpathfromphoto.jpg");
			} else {
				img = selectedImage;
			}

			// 64 is already a BIG value
			return ImageFilters.Filter (img, 128);
		}

		UIButton CreateLevelButton (int x, int y, int width, int height, PuzzleData puzzle)
		{
			// Load the image
			UIImage image = UIImage.FromFile (puzzle.Filename);
			UIButton levelButton = new UIButton (new RectangleF (x, y, width, height));
			levelButton.SetImage (image, UIControlState.Normal);
			levelButton.BackgroundColor = UIColor.White;
			levelButton.Layer.BorderColor = UIColor.Blue.CGColor;
			levelButton.Layer.BorderWidth = 3f;
			levelButton.TouchUpInside += (object sender, EventArgs e) => {
				string puzzleFilename = puzzle.Filename;
				launchLevel (puzzle, UIImage.FromFile (puzzleFilename));
			};

			return levelButton;
		}

		private void showButtonPanel ()
		{
			levelSelectionPanel.RemoveFromSuperview ();
			View.AddSubview (buttonPanel);
		}

		private void showLevelSelection ()
		{
			buttonPanel.RemoveFromSuperview ();
			View.AddSubview (levelSelectionPanel);
		}

		private void showCapturedImage (UIImage image)
		{
			BeginInvokeOnMainThread (() => {
				UIImageView imageView = new UIImageView (image);
				View.Add (imageView);
			});
		}

		private void launchLevel (PuzzleData puzzle, UIImage level)
		{
			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 

			foreach (var v in View.Subviews) {
				v.RemoveFromSuperview ();
			}

			Logger.I ("Launching level!");

			appDelegate.ShowPuzzle (puzzle, level);
		}
	}
}

