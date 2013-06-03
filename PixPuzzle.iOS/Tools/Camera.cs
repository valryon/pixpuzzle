using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

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
		static UIPopoverController popOver;

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
				cb (info);
			}
		}

		public static void TakePicture (UIViewController parent, Action<NSDictionary> callback)
		{
			displayPicker (UIImagePickerControllerSourceType.Camera, parent, callback);
		}

		public static void SelectPicture (UIViewController parent, Action<NSDictionary> callback)
		{
			displayPicker (UIImagePickerControllerSourceType.PhotoLibrary, parent, callback);
		}

		private static void displayPicker (UIImagePickerControllerSourceType source, UIViewController parent, Action<NSDictionary> callback)
		{
			Init ();
			picker.SourceType = source;
			picker.AllowsEditing = true;
			_callback = callback;

			if (popOver == null || popOver.ContentViewController == null) { 
				popOver = new UIPopoverController (picker); 
			} 
			popOver.PresentFromRect (parent.View.Frame, parent.View, UIPopoverArrowDirection.Any, true); 
		}
	}
}

