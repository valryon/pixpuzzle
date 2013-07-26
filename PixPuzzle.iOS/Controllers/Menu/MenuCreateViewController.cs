using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PixPuzzle
{
	public partial class MenuCreateViewController : UIViewController
	{
		public MenuCreateViewController (IntPtr handle)
			: base(handle)
		{
		}

		partial void OnTakePictureButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var vc = this.Storyboard.InstantiateViewController("MenuCreateStep2ViewController") as UIViewController;

			NavigationController.PushViewController(
				vc,
				true
				);
		}
	}
}

