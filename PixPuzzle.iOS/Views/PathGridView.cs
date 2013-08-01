using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using PixPuzzle.Data;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
using MonoTouch.Foundation;

namespace PixPuzzle
{
	internal class PathGridViewInternal : UIView, IPathGridView
	{
		private PathGridView mParent;
		private UIImage mSplashImage, mSplashValidImage, mPathImage;
		private CGContext mContext;
		private Rectangle mDrawRect;

		public PathGridViewInternal (PathGridView parent, RectangleF frame) 
			: base(frame)
		{
			this.mParent = parent;

			// -- Gestures
			// ---- Double tap to delete
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (doubleTapEvent);
			tapGesture.NumberOfTapsRequired = 2;
			this.AddGestureRecognizer (tapGesture);
		}
	
		#region iOS Drawing

		public void InitializeViewForDrawing ()
		{
			// Find the real final size and location of the frame
			this.Frame = new RectangleF (mParent.GridLocation.X, mParent.GridLocation.Y
			                             , (mParent.CellSize * mParent.Width) + mParent.GridLocation.X + mParent.BorderWidth
			                             , (mParent.CellSize * mParent.Height) + mParent.GridLocation.Y + mParent.BorderWidth
			                             );

			BackgroundColor = UIColor.FromPatternImage(new UIImage("grid_background.png"));

			mSplashImage = new UIImage ("splash.png");
			mSplashValidImage = new UIImage ("splash_valid.png");
			mPathImage = new UIImage ("path.png");
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			// Save the rect
			this.mDrawRect = new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

			// Order the big draw
			mParent.DrawPuzzle ();
		}

		public void OrderRefresh (Rectangle zoneToRefresh)
		{
			// iOS Specific
			SetNeedsDisplayInRect (zoneToRefresh);
		}

		public void StartDraw ()
		{
			// Context properties
			this.mContext = UIGraphics.GetCurrentContext ();

			// -- Text
			mContext.SetTextDrawingMode (CGTextDrawingMode.Fill);
			mContext.TextMatrix = CGAffineTransform.MakeScale (1f, -1f);

		}

		public bool IsToRefresh (PathCell cell, Rectangle cellRect)
		{
			return mDrawRect.IntersectsWith (cellRect) || mDrawRect.Contains (cellRect);
		}

		public void DrawGrid ()
		{
			Point borderStartLocation = mParent.BorderStartLocation;
			int borderWidth = mParent.BorderWidth;

			// Draw the borders of the grid
			// ------------------------------------------------------------
			int borderEndX = borderStartLocation.X + (mParent.CellSize * mParent.Width) + (borderWidth / 2);
			int borderEndY = borderStartLocation.Y + (mParent.CellSize * mParent.Height) + (borderWidth / 2);

			mContext.SetStrokeColorWithColor (UIColor.Black.CGColor);
			mContext.MoveTo (borderStartLocation.X, borderStartLocation.Y); //start at this point
			mContext.SetLineWidth (borderWidth);

			// Draw a square
			mContext.AddLineToPoint (borderEndX, borderStartLocation.Y); 
			mContext.AddLineToPoint (borderEndX, borderEndY); 
			mContext.AddLineToPoint (borderStartLocation.X, borderEndY); 
			mContext.AddLineToPoint (borderStartLocation.X, borderStartLocation.Y); 

			// Effective draw
			mContext.StrokePath ();

			// Draw cells 
			// ------------------------------------------------------------
			mContext.SetStrokeColorWithColor (UIColor.Black.CGColor);
			mContext.SetLineWidth (1.0f);
			mContext.SetLineDash (0.5f, new float[] { 4, 2 });
			mContext.MoveTo (borderStartLocation.X, borderStartLocation.X);

			for (int x=0; x<mParent.Width; x++) {
				int cellBorderX = borderStartLocation.X + x * mParent.CellSize;
				mContext.MoveTo (cellBorderX, borderStartLocation.X);
				mContext.AddLineToPoint (cellBorderX, borderStartLocation.Y + mParent.Height * mParent.CellSize);
			}

			for (int y=0; y<mParent.Height; y++) {
				int cellBorderY = borderStartLocation.X + y * mParent.CellSize;
				mContext.MoveTo (borderStartLocation.X, cellBorderY);
				mContext.AddLineToPoint (borderStartLocation.X + mParent.Width * mParent.CellSize, cellBorderY);
			}

			mContext.StrokePath ();
		}

		public void DrawCellBase (PathCell cell, Rectangle rectangle)
		{
			bool isValid = cell.Path != null && cell.Path.IsValid;
			CellColor cellColor = cell.Color;

			if (cell.Path != null) {
				cellColor = cell.Path.Color;
			}

			CGColor color = cellColor.UIColor.CGColor;
			mContext.SetFillColor (color);

			if (mParent.ShouldDisplayFilledCells == false) {
				// Draw a circle of the color
				// But reduce the circle value
				int circleReductionValue = mParent.CellSize / 10;

				RectangleF cellValueRect = new RectangleF (rectangle.X + circleReductionValue, rectangle.Y + circleReductionValue, mParent.CellSize - 2 * circleReductionValue, mParent.CellSize - 2 * circleReductionValue);

				UIImage image = null;

				if (isValid == false) {
					image = mSplashImage;
				} else {
					image = mSplashValidImage;
				}

				image = UIImageEx.GetImageWithOverlayColor (image, cellColor.UIColor);

				mContext.DrawImage (cellValueRect, image.CGImage);
			} else {
				// Fill the whole cell to preview puzzle
				mContext.FillRect (rectangle);
			}
		}

		public void DrawPath (PathCell cell, Rectangle pathRect, Point direction, CellColor color)
		{
			mContext.DrawImage (pathRect, UIImageEx.GetImageWithOverlayColor(mPathImage, color.UIColor).CGImage);

			// The old drawing code for perfect paths
//			context.SetFillColor (color.UIColor.CGColor);

			// Draw in 2 parts:
			// First a rect
//			context.FillRect (pathRect);
//
//			// Then an arc to the end
//			if (direction.X < 0) {
//				context.MoveTo (pathRect.Right, pathRect.Top);
//				context.AddCurveToPoint (
//					pathRect.Right + pathRect.Width / 3,
//					pathRect.Top + pathRect.Height / 3,
//					pathRect.Right + pathRect.Width / 3,
//					pathRect.Top + 2 * pathRect.Height / 3,
//					pathRect.Right,
//					pathRect.Bottom
//					);
//			} else if (direction.X > 0) {
//				context.MoveTo (pathRect.Left, pathRect.Top);
//				context.AddCurveToPoint (
//					pathRect.Left - pathRect.Width / 3,
//					pathRect.Top + pathRect.Height / 3,
//					pathRect.Left - pathRect.Width / 3,
//					pathRect.Top + 2 * pathRect.Height / 3,
//					pathRect.Left,
//					pathRect.Bottom
//					);
//			}
//			if (direction.Y < 0) {
//				context.MoveTo (pathRect.Left, pathRect.Bottom);
//				context.AddCurveToPoint (
//					pathRect.Left + pathRect.Width / 3,
//					pathRect.Bottom + pathRect.Height / 3,
//					pathRect.Left + 2 * pathRect.Width / 3,
//					pathRect.Bottom + pathRect.Height / 3,
//					pathRect.Right,
//					pathRect.Bottom
//					);
//			} else if (direction.Y > 0) {
//				context.MoveTo (pathRect.Left, pathRect.Top);
//				context.AddCurveToPoint (
//					pathRect.Left + pathRect.Width / 3,
//					pathRect.Top - pathRect.Height / 3,
//					pathRect.Left + 2 * pathRect.Width / 3,
//					pathRect.Top - pathRect.Height / 3,
//					pathRect.Right,
//					pathRect.Top
//					);
//			}
//
//			context.FillPath ();
		}

		public void DrawLastCellIncompletePath (PathCell cell, Rectangle rect, string pathValue, CellColor color)
		{
			UIColor colorUnderText = UIColor.LightGray;

			// Draw a gray circle!
			int circleReductionValue = mParent.CellSize / 8;
			mContext.SetFillColor (colorUnderText.CGColor);
			mContext.FillEllipseInRect (new RectangleF(rect.X + circleReductionValue, rect.Y + circleReductionValue, mParent.CellSize-2*circleReductionValue, mParent.CellSize-2*circleReductionValue));

			// Draw the text
			mContext.SetFillColor (UIColor.Black.CGColor);
			mContext.SelectFont ("Helvetica Neue", 12.0f, CGTextEncoding.MacRoman);
			mContext.ShowTextAtPoint (rect.X + mParent.CellSize/3, rect.Y + 2 * mParent.CellSize / 3, pathValue);
		}

		public void DrawCellText (PathCell cell, Rectangle location, string text, CellColor color)
		{
			mContext.SelectFont ("Helvetica Neue", 16.0f, CGTextEncoding.MacRoman);

			// Get the reverse color of the background
			float sumOfComponents = 0;
			float r, g, b, a;
			color.UIColor.GetRGBA (out r, out g, out b, out a);
			sumOfComponents = r + g + b;

			UIColor textColor = UIColor.Black;
			if (sumOfComponents < 0.25f) 
			{
				textColor = UIColor.White;
			} else {
				textColor = UIColor.Black;
			}

			mContext.SetFillColor (textColor.CGColor);

			// Careful with the coordinates!!!
			// Remember it's a real mess because it's inverted
			mContext.ShowTextAtPoint (location.X + mParent.CellSize/3, location.Y + 2 * mParent.CellSize / 3, text);
		}

		public void EndDraw ()
		{

		}

		#endregion

		#region Grid tools

		private PathCell getCellFromViewCoordinates (PointF viewLocation)
		{
			int x = (int)(viewLocation.X / (float)mParent.CellSize);
			int y = (int)(viewLocation.Y / (float)mParent.CellSize);

			return mParent.GetCell (x, y);
		}
		#endregion

		#region Events

		private void doubleTapEvent (UIGestureRecognizer sender)
		{
			// Disable gestures on preview mode
			if (mParent.ShouldDisplayFilledCells)
				return; 

			if (sender.NumberOfTouches > 0) {

				PointF touchLocation = sender.LocationInView (this);

				PathCell cell = getCellFromViewCoordinates (touchLocation);
				mParent.RemovePath (cell);
			}
		}

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Disable gestures on preview mode
			if (mParent.ShouldDisplayFilledCells)
				return; 

			// Touch began: find the cell under the finger and register it as a path start
			if (touches.Count == 1) {
				UITouch touch = (UITouch)touches.AnyObject;

				PointF fingerLocation = touch.LocationInView (this);

				PathCell cell = getCellFromViewCoordinates (fingerLocation);

				mParent.StartPathCreation (cell);
			}
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Create a path under the finger following the grid
			if (touches.Count == 1) {

				// Valid movement
				if (mParent.IsCreatingPath) {
					UITouch touch = (UITouch)touches.AnyObject;

					PointF fingerLocation = touch.LocationInView (this);

					PathCell cell = getCellFromViewCoordinates (fingerLocation);

					mParent.CreatePath (cell);
				}
			}
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			mParent.EndPathCreation (false);

			base.TouchesEnded (touches, evt);
		}
		#endregion

		protected override void Dispose (bool disposing)
		{
			mParent = null;
			base.Dispose (disposing);
		}
	}
	/// <summary>
	/// Grid iOS view.
	/// </summary>
	public class PathGridView : Grid
	{
		// Constants
		public const int CELL_SIZE_IPHONE = 32;
		public const int CELL_SIZE_IPAD = 48;

		public PathGridView (PuzzleData puzzle, int width, int height)
			: base(puzzle, width, height, AppDelegate.UserInterfaceIdiomIsPhone ? CELL_SIZE_IPHONE : CELL_SIZE_IPAD)
		{
			// Create the view 
			PathGridViewInternal theView = new PathGridViewInternal (this, new RectangleF (0, 0, width * CellSize, height * CellSize));

			// Create the grid and cells views
			CreateGrid (0,0, theView);

			GridViewInternal = theView;
		}

		internal PathGridViewInternal GridViewInternal {
			get;
			private set;
		}
	}
}

