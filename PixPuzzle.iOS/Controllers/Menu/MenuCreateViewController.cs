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
			Camera.TakePicture (this, (dico) => {

				UIImage img = null;

				// Get camera result
				var selectedImageObject = dico.ObjectForKey (UIImagePickerController.OriginalImage);
				
				if (selectedImageObject != null && selectedImageObject is UIImage) {
					img = selectedImageObject as UIImage;
				}
				GoToStep2 (img);
			});
		}

		partial void OnLibraryButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			Camera.SelectPicture (this, (dico) => {

				UIImage img = null;

				// Get camera result
				var selectedImageObject = dico.ObjectForKey (UIImagePickerController.OriginalImage);

				if (selectedImageObject != null && selectedImageObject is UIImage) {
					img = selectedImageObject as UIImage;
				}
				GoToStep2 (img);
			});
		}

		private void GoToStep2 (UIImage img)
		{
			var vc = this.Storyboard.InstantiateViewController ("MenuCreateStep2ViewController") as UIViewController;

			NavigationController.PushViewController (
				vc,
				true
			);
		}
	}
}

