using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using PixPuzzle.Data;

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
			View = new UIView (new RectangleF(0,0,UIScreen.MainScreen.Bounds.Height,UIScreen.MainScreen.Bounds.Width));
			View.BackgroundColor = UIColor.FromPatternImage (new UIImage("background.png"));

			// First buttons
			// --------------------------------
			buttonPanel = new UIView (new RectangleF(0,0,UIScreen.MainScreen.Bounds.Height,UIScreen.MainScreen.Bounds.Width));

			int buttonWidth = 300;
			int buttonHeight = 80;
			RectangleF buttonRect = new RectangleF (UIScreen.MainScreen.Bounds.Height/2 - buttonWidth/2,
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
			levelSelectionPanel = new UIView (new RectangleF(0,0,UIScreen.MainScreen.Bounds.Height,UIScreen.MainScreen.Bounds.Width));
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

			foreach (var puzzle in Directory.GetFiles(GameViewController.ImageDirectory)) 
			{
				UIButton levelButton = CreateLevelButton(x,y,width, height, puzzle);
			
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

			UIButton pictureButton = new UIButton (new RectangleF (View.Frame.Width / 2 - width, y, width * 2, height / 2));
			pictureButton.BackgroundColor = UIColor.White;
			pictureButton.Layer.BorderColor = UIColor.Green.CGColor;
			pictureButton.Layer.BorderWidth = 3f;
			pictureButton.SetTitleColor (UIColor.Black, UIControlState.Normal);
			pictureButton.SetTitle ("Custom photo", UIControlState.Normal);
			pictureButton.TouchUpInside += (object sender, EventArgs e) =>  {
				Camera.TakePicture(this, (dico) => {
//				Camera.SelectPicture(this, (dico) => {
//					showCapturedImage(new UIImage()); // TODO One day

				});
			};

			scroll.AddSubview (pictureButton);

			scrolLContentSize.Height += baseY;
			scroll.ContentSize = new SizeF (scrolLContentSize.Width, scrolLContentSize.Height);
		}

		UIButton CreateLevelButton (int x, int y, int width, int height, string puzzle)
		{
			// Load the image
			UIImage image = UIImage.FromFile (puzzle);
			UIButton levelButton = new UIButton (new RectangleF (x, y, width, height));
			levelButton.SetImage (image, UIControlState.Normal);
			levelButton.BackgroundColor = UIColor.White;
			levelButton.Layer.BorderColor = UIColor.Blue.CGColor;
			levelButton.Layer.BorderWidth = 3f;
			levelButton.TouchUpInside += (object sender, EventArgs e) =>  {
				string puzzleFilename = puzzle;
				launchLevel (puzzleFilename);
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

		private void launchLevel(string level) 
		{
			UIAlertView alert = new UIAlertView (
				"Game Mode",
				"Choisir le mode de jeu",
				null,
				"Retour", "Path", "Picross");

			alert.Dismissed += (object sender, UIButtonEventArgs e) => {

				var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate; 

				if(e.ButtonIndex == 1) {
					GameModes mode = GameModes.Path;
					appDelegate.ShowPuzzle (mode,level);
				}
				else if(e.ButtonIndex == 2) {
					GameModes mode = GameModes.Picross;
					appDelegate.ShowPuzzle (mode,level);
				}
			};
			alert.Show ();

		}
	}
}

