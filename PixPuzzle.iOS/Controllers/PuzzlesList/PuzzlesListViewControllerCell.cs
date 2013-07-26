using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PixPuzzle
{
	public class PuzzlesListViewControllerCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("PuzzlesListViewControllerCell");

		[Export ("initWithFrame:")]
		public PuzzlesListViewControllerCell (RectangleF frame) : base (frame)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			BackgroundColor = UIColor.Cyan;
		}
	}
}

