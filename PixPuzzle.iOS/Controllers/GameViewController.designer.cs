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
	[Register ("GameViewController")]
	partial class GameViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ButtonPause { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelTime { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollViewGame { get; set; }

		[Action ("OnButtonPausePressed:")]
		partial void OnButtonPausePressed (MonoTouch.Foundation.NSObject sender);

		[Action ("OnButtonQuitPressed:")]
		partial void OnButtonQuitPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LabelTime != null) {
				LabelTime.Dispose ();
				LabelTime = null;
			}

			if (ScrollViewGame != null) {
				ScrollViewGame.Dispose ();
				ScrollViewGame = null;
			}

			if (ButtonPause != null) {
				ButtonPause.Dispose ();
				ButtonPause = null;
			}
		}
	}
}
