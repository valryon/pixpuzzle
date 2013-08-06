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
	[Register ("MenuViewController")]
	partial class MenuViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ButtonCreate { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ButtonCredits { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ButtonFriends { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ButtonPlay { get; set; }

		[Action ("OnCreateButtonPressed:")]
		partial void OnCreateButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("OnCreditsButtonPressed:")]
		partial void OnCreditsButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("OnFriendsButtonPressed:")]
		partial void OnFriendsButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("OnPlayButtonPressed:")]
		partial void OnPlayButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ButtonPlay != null) {
				ButtonPlay.Dispose ();
				ButtonPlay = null;
			}

			if (ButtonCreate != null) {
				ButtonCreate.Dispose ();
				ButtonCreate = null;
			}

			if (ButtonFriends != null) {
				ButtonFriends.Dispose ();
				ButtonFriends = null;
			}

			if (ButtonCredits != null) {
				ButtonCredits.Dispose ();
				ButtonCredits = null;
			}
		}
	}
}
