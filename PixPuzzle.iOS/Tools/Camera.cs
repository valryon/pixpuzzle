using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace PixPuzzle
{
	//
	// A static class that will reuse the UIImagePickerController
	// as iPhoneOS has a crash if multiple UIImagePickerController are created
	//   http://stackoverflow.com/questions/487173
	// (Follow the links)
	//
	public static class Camera
	{
		static UIImagePickerController picker;
		static Action<NSDictionary> _callback;
		static UIPopoverController popover;

		static void Init ()
		{
			if (picker != null)
				return;

			picker = new UIImagePickerController ();
			picker.Delegate = new CameraDelegate ();
		}

		class CameraDelegate : UIImagePickerControllerDelegate
		{
			public override void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info)
			{
				var cb = _callback;
				_callback = null;

				picker.DismissViewController (true, null);
				popover.Dismiss (true);

				cb (info);
			}
		}

		public static void TakePicture (UIViewController parent, Action<NSDictionary> callback)
		{
			Init ();
			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			_callback = callback;

			if (AppDelegate.UserInterfaceIdiomIsPhone == false) {
				popover = new UIPopoverController (picker);
				popover.PresentFromRect (new RectangleF (150, 150, 500, 500), parent.View, UIPopoverArrowDirection.Any, true);
			} else {
				parent.PresentViewController (picker, true, null);
			}
		}

		public static void SelectPicture (UIViewController parent, Action<NSDictionary> callback)
		{
			Init ();
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			_callback = callback;

			if (AppDelegate.UserInterfaceIdiomIsPhone == false) {
				popover = new UIPopoverController (picker);
				popover.PresentFromRect (new RectangleF (150, 150, 500, 500), parent.View, UIPopoverArrowDirection.Any, true);
			} else {
				parent.PresentViewController (picker, true, null);
			}
		}
	}
}

