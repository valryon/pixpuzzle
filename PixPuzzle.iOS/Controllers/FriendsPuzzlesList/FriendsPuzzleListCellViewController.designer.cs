// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace PixPuzzle
{
	partial class FriendsPuzzleListCellViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ButtonDelete { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImagePlayer1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImagePlayer2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImagePuzzle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelPlayer1Time { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelPlayer2Time { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ImagePuzzle != null) {
				ImagePuzzle.Dispose ();
				ImagePuzzle = null;
			}

			if (ButtonDelete != null) {
				ButtonDelete.Dispose ();
				ButtonDelete = null;
			}

			if (LabelPlayer1Time != null) {
				LabelPlayer1Time.Dispose ();
				LabelPlayer1Time = null;
			}

			if (LabelPlayer2Time != null) {
				LabelPlayer2Time.Dispose ();
				LabelPlayer2Time = null;
			}

			if (ImagePlayer1 != null) {
				ImagePlayer1.Dispose ();
				ImagePlayer1 = null;
			}

			if (ImagePlayer2 != null) {
				ImagePlayer2.Dispose ();
				ImagePlayer2 = null;
			}
		}
	}
}
