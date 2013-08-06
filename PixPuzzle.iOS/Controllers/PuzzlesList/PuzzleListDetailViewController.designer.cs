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
	[Register ("PuzzleListDetailViewController")]
	partial class PuzzleListDetailViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ButtonPlay { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImagePuzzle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelSize { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelTime { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LabelTitle { get; set; }

		[Action ("OnButtonPlayPressed:")]
		partial void OnButtonPlayPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ImagePuzzle != null) {
				ImagePuzzle.Dispose ();
				ImagePuzzle = null;
			}

			if (LabelTitle != null) {
				LabelTitle.Dispose ();
				LabelTitle = null;
			}

			if (LabelSize != null) {
				LabelSize.Dispose ();
				LabelSize = null;
			}

			if (LabelTime != null) {
				LabelTime.Dispose ();
				LabelTime = null;
			}

			if (ButtonPlay != null) {
				ButtonPlay.Dispose ();
				ButtonPlay = null;
			}
		}
	}
}
