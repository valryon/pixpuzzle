using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PixPuzzle
{
	public partial class MenuCreateStep2ViewController : UIViewController
	{
		public MenuCreateStep2ViewController (IntPtr handle) : base (handle)
		{
		}

		partial void OnPlayButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var vc = this.Storyboard.InstantiateViewController("GameViewController") as UIViewController;

			NavigationController.PushViewController(
				vc,
				true
				);
		}
	}
}

