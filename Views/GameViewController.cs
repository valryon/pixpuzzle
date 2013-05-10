
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace PixPuzzle
{
	public partial class GameViewController : UIViewController
	{
		private GameGrid grid;

		public GameViewController ()
			: base (null, null)
		{
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
//			UIImage image = UIImage.FromFile ("mario.png");
			UIImage image = UIImage.FromFile ("kirby.jpg");
			Bitmap bitmap = new Bitmap(image);

			grid = new GameGrid((int)image.Size.Width,(int)image.Size.Height);

			UIScrollView scrollView = new UIScrollView(View.Frame);
			scrollView.ScrollEnabled = true;
			scrollView.BackgroundColor = UIColor.Gray;
			scrollView.AddSubview(grid);
			View.AddSubview (scrollView);

			// Look at each pixel
			for(int x=0;x<image.Size.Width;x++) {
				for(int y=0;y<image.Size.Height;y++) {

					// Get the pixel color
					Color c = bitmap.GetPixel(x,y);
					grid.SetPixelData(x,y,new UIColor(c.R/255f, c.G/255f, c.B/255f, c.A/255f));
				}	
			}


		}
	}
}

