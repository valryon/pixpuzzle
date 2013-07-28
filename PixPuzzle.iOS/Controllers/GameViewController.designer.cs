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
		MonoTouch.UIKit.UIScrollView ScrollViewGame { get; set; }

		[Action ("OnButtonQuitPressed:")]
		partial void OnButtonQuitPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ScrollViewGame != null) {
				ScrollViewGame.Dispose ();
				ScrollViewGame = null;
			}
		}
	}
}
