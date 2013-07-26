using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PixPuzzle
{
	[Register ("MenuPlayViewController")]
	public class MenuPlayViewController : UICollectionViewController
	{
		public MenuPlayViewController (IntPtr handle) : base (handle)
		{
		}

		public MenuPlayViewController (UICollectionViewLayout layout) : base (layout)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register any custom UICollectionViewCell classes
			CollectionView.RegisterClassForCell (typeof(PuzzlesListViewControllerCell), PuzzlesListViewControllerCell.Key);
			
			// Note: If you use one of the Collection View Cell templates to create a new cell type,
			// you can register it using the RegisterNibForCell() method like this:
			//
			// CollectionView.RegisterNibForCell (MyCollectionViewCell.Nib, MyCollectionViewCell.Key);
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			// TODO: return the actual number of items in the section
			return 1;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (PuzzlesListViewControllerCell.Key, indexPath) as PuzzlesListViewControllerCell;
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			
			return cell;
		}

//		public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
//		{
			// TODO
//			return base.GetViewForSupplementaryElement (collectionView, elementKind, indexPath);
//		}
	}
}

