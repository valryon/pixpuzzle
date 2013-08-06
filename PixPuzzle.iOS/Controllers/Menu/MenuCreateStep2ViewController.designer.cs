// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace PixPuzzle
{
	[Register ("MenuCreateStep2ViewController")]
	partial class MenuCreateStep2ViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ButtonPlayPressed { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ButtonShare { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImageTransformed { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider SliderDifficulty { get; set; }

		[Action ("OnPlayButtonPressed:")]
		partial void OnPlayButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("OnShareButtonPressed:")]
		partial void OnShareButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ButtonPlayPressed != null) {
				ButtonPlayPressed.Dispose ();
				ButtonPlayPressed = null;
			}

			if (ButtonShare != null) {
				ButtonShare.Dispose ();
				ButtonShare = null;
			}

			if (ImageTransformed != null) {
				ImageTransformed.Dispose ();
				ImageTransformed = null;
			}

			if (SliderDifficulty != null) {
				SliderDifficulty.Dispose ();
				SliderDifficulty = null;
			}
		}
	}
}
