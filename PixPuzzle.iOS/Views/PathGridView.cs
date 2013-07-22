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
		private PathGridView parent;
		private UIImage splashImage, splashValidImage, pathImage;

		public PathGridViewInternal (PathGridView parent, RectangleF frame) 
			: base(frame)
		{
			this.parent = parent;

			// -- Gestures
			// ---- Double tap to delete
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (doubleTapEvent);
			tapGesture.NumberOfTapsRequired = 2;
			this.AddGestureRecognizer (tapGesture);
		}
	
		#region iOS Drawing

		private CGContext context;
		private Rectangle drawRect;

		public void InitializeViewForDrawing ()
		{
			// Find the real final size and location of the frame
			this.Frame = new RectangleF (parent.GridLocation.X, parent.GridLocation.Y
			                             , (parent.CellSize * parent.Width) + parent.GridLocation.X + parent.BorderWidth
			                             , (parent.CellSize * parent.Height) + parent.GridLocation.Y + parent.BorderWidth
			                             );

			BackgroundColor = UIColor.FromPatternImage(new UIImage("grid_background.png"));

			splashImage = new UIImage ("splash.png");
			splashValidImage = new UIImage ("splash_valid.png");
			pathImage = new UIImage ("path.png");
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			// Save the rect
			this.drawRect = new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

			// Order the big draw
			parent.DrawPuzzle ();
		}

		public void OrderRefresh (Rectangle zoneToRefresh)
		{
			// iOS Specific
			SetNeedsDisplayInRect (zoneToRefresh);
		}

		public void StartDraw ()
		{
			// Context properties
			this.context = UIGraphics.GetCurrentContext ();

			// -- Text
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.TextMatrix = CGAffineTransform.MakeScale (1f, -1f);

		}

		public bool IsToRefresh (PathCell cell, Rectangle cellRect)
		{
			return drawRect.IntersectsWith (cellRect) || drawRect.Contains (cellRect);
		}

		public void DrawGrid ()
		{
			Point borderStartLocation = parent.BorderStartLocation;
			int borderWidth = parent.BorderWidth;

			// Draw the borders of the grid
			// ------------------------------------------------------------
			int borderEndX = borderStartLocation.X + (parent.CellSize * parent.Width) + (borderWidth / 2);
			int borderEndY = borderStartLocation.Y + (parent.CellSize * parent.Height) + (borderWidth / 2);

			context.SetStrokeColorWithColor (UIColor.Black.CGColor);
			context.MoveTo (borderStartLocation.X, borderStartLocation.Y); //start at this point
			context.SetLineWidth (borderWidth);

			// Draw a square
			context.AddLineToPoint (borderEndX, borderStartLocation.Y); 
			context.AddLineToPoint (borderEndX, borderEndY); 
			context.AddLineToPoint (borderStartLocation.X, borderEndY); 
			context.AddLineToPoint (borderStartLocation.X, borderStartLocation.Y); 

			// Effective draw
			context.StrokePath ();

			// Draw cells 
			// ------------------------------------------------------------
			context.SetStrokeColorWithColor (UIColor.Black.CGColor);
			context.SetLineWidth (1.0f);
			context.SetLineDash (0.5f, new float[] { 4, 2 });
			context.MoveTo (borderStartLocation.X, borderStartLocation.X);

			for (int x=0; x<parent.Width; x++) {
				int cellBorderX = borderStartLocation.X + x * parent.CellSize;
				context.MoveTo (cellBorderX, borderStartLocation.X);
				context.AddLineToPoint (cellBorderX, borderStartLocation.Y + parent.Height * parent.CellSize);
			}

			for (int y=0; y<parent.Height; y++) {
				int cellBorderY = borderStartLocation.X + y * parent.CellSize;
				context.MoveTo (borderStartLocation.X, cellBorderY);
				context.AddLineToPoint (borderStartLocation.X + parent.Width * parent.CellSize, cellBorderY);
			}

			context.StrokePath ();
		}

		public void DrawCellBase (PathCell cell, Rectangle rectangle)
		{
			bool isValid = cell.Path != null && cell.Path.IsValid;
			CellColor cellColor = cell.Color;

			if (cell.Path != null) {
				cellColor = cell.Path.Color;
			}

			CGColor color = cellColor.UIColor.CGColor;
			context.SetFillColor (color);

			// Draw a circle of the color
			// But reduce the circle value
			int circleReductionValue = parent.CellSize / 10;

			RectangleF cellValueRect = new RectangleF (rectangle.X + circleReductionValue, rectangle.Y + circleReductionValue, parent.CellSize - 2 * circleReductionValue, parent.CellSize - 2 * circleReductionValue);

			UIImage image = null;

			if (isValid == false) {
				image = splashImage;
			} else {
				image = splashValidImage;
			}

			image = UIImageEx.GetImageWithOverlayColor (image, cellColor.UIColor);

			context.DrawImage (cellValueRect, image.CGImage);
		}

		public void DrawPath (PathCell cell, Rectangle pathRect, Point direction, CellColor color)
		{
			context.DrawImage (pathRect, UIImageEx.GetImageWithOverlayColor(pathImage, color.UIColor).CGImage);

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
			int circleReductionValue = parent.CellSize / 8;
			context.SetFillColor (colorUnderText.CGColor);
			context.FillEllipseInRect (new RectangleF(rect.X + circleReductionValue, rect.Y + circleReductionValue, parent.CellSize-2*circleReductionValue, parent.CellSize-2*circleReductionValue));

			// Draw the text
			context.SetFillColor (UIColor.Black.CGColor);
			context.SelectFont ("Helvetica Neue", 12.0f, CGTextEncoding.MacRoman);
			context.ShowTextAtPoint (rect.X + parent.CellSize/3, rect.Y + 2 * parent.CellSize / 3, pathValue);
		}

		public void DrawCellText (PathCell cell, Rectangle location, string text, CellColor color)
		{
			context.SelectFont ("Helvetica Neue", 16.0f, CGTextEncoding.MacRoman);

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

			context.SetFillColor (textColor.CGColor);

			// Careful with the coordinates!!!
			// Remember it's a real mess because it's inverted
			context.ShowTextAtPoint (location.X + parent.CellSize/3, location.Y + 2 * parent.CellSize / 3, text);
		}

		public void EndDraw ()
		{

		}

		#endregion

		#region Grid tools

		private PathCell getCellFromViewCoordinates (PointF viewLocation)
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

				PathCell cell = getCellFromViewCoordinates (touchLocation);
				parent.RemovePath (cell);
			}
		}

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Touch began: find the cell under the finger and register it as a path start
			if (touches.Count == 1) {
				UITouch touch = (UITouch)touches.AnyObject;

				PointF fingerLocation = touch.LocationInView (this);

				Console.WriteLine (fingerLocation);

				PathCell cell = getCellFromViewCoordinates (fingerLocation);

				parent.StartPathCreation (cell);
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

					PathCell cell = getCellFromViewCoordinates (fingerLocation);

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
	public class PathGridView : PathGrid
	{
		// Constants
		public const int CellSizeIphone = 32;
		public const int CellSizeIpad = 48;

		public PathGridView (PuzzleData puzzle, int width, int height)
			: base(puzzle, width, height, AppDelegate.UserInterfaceIdiomIsPhone ? CellSizeIphone : CellSizeIpad)
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

