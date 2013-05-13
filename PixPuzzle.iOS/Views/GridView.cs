using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using PixPuzzle.Data;

namespace PixPuzzle
{
	internal class GridViewInternal : UIView
	{
		private GridView parent;

		public GridViewInternal(GridView parent, RectangleF frame) 
			: base(frame)
		{
			this.parent = parent;

			// -- Gestures
			// ---- Double tap to delete
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (doubleTapEvent);
			tapGesture.NumberOfTapsRequired = 2;
			this.AddGestureRecognizer (tapGesture);
		}

		#region Grid tools

		private Cell getCellFromViewCoordinates (PointF viewLocation)
		{
			int x = (int)(viewLocation.X / (float)parent.CellSize);
			int y = (int)(viewLocation.Y / (float)parent.CellSize);

			return parent.GetCell (x, y);
		}
		#endregion

		#region Events

		private void doubleTapEvent (UIGestureRecognizer sender)
		{
			if (sender.NumberOfTouches > 0) {

				PointF touchLocation = sender.LocationInView (this);

				Cell cell = getCellFromViewCoordinates (touchLocation);

				parent.RemovePath (cell);
			}
		}

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Touch began: find the cell under the finger and register it as a path start
			if (touches.Count == 1) {
				UITouch touch = (UITouch)touches.AnyObject;

				PointF fingerLocation = touch.LocationInView (this);

				Cell cell = getCellFromViewCoordinates (fingerLocation);

				bool pathStarted = parent.StartPathCreation (cell);

				if (pathStarted) {
					this.BringSubviewToFront (((CellView)cell).View);
				}
			}
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Create a path under the finger following the grid
			if (touches.Count == 1) {

				// Valid movement
				if (parent.IsCreatingPath) {
					UITouch touch = (UITouch)touches.AnyObject;

					PointF fingerLocation = touch.LocationInView (this);

					Cell cell = getCellFromViewCoordinates (fingerLocation);

					parent.CreatePath (cell);
				}
			}
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			parent.EndPathCreation (false);

			base.TouchesEnded (touches, evt);
		}
		#endregion

		protected override void Dispose (bool disposing)
		{
			parent = null;
			base.Dispose (disposing);
		}
	}

	/// <summary>
	/// Grid iOS view.
	/// </summary>
	public class GridView : Grid<CellView>
	{
		// Constants
		public const int CellSizeIphone = 32;
		public const int CellSizeIpad = 48;

		public GridView (int width, int height)
			: base(width, height, AppDelegate.UserInterfaceIdiomIsPhone ? CellSizeIphone : CellSizeIpad)
		{
			// Create the view 
			View = new GridViewInternal(this, new RectangleF (0, 0, width * CellSize, height * CellSize));

			// Create the grid and cells views
			CreateGrid ((x,y) => {
				CellView cell = new CellView(x,y, new RectangleF (x * CellSize, y * CellSize, CellSize, CellSize));

				cell.BuildView();

				View.AddSubview(cell.View);

				return cell;
			});
		}

		public UIView View
		{
			get;set;
		}
	}
}

