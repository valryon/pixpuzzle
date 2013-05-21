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
	internal class GridViewInternal : UIView
	{
		private GridView parent;

		public GridViewInternal (GridView parent, RectangleF frame) 
			: base(frame)
		{
			this.parent = parent;

			// -- Gestures
			// ---- Double tap to delete
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (doubleTapEvent);
			tapGesture.NumberOfTapsRequired = 2;
			this.AddGestureRecognizer (tapGesture);
		}
	
		#region Generic Drawing

		private const int BorderWidth = 4;
		private const int GridLocationX = 0;
		private const int GridLocationY = 0;
		private Point BorderStartLocation;
		private UIColor defaultBackgroundColor;

		public virtual void InitializeViewForDrawing (int x, int y)
		{
			// Find the real final size and location of the frame
			this.Frame = new RectangleF (x, y
			                            , (parent.CellSize * parent.Width) + GridLocationX + BorderWidth
			                            , (parent.CellSize * parent.Height) + GridLocationY + BorderWidth
			);

			defaultBackgroundColor = UIColor.FromRGB (230, 230, 230);
			this.BackgroundColor = defaultBackgroundColor;

			int borderStartX = GridLocationX + (BorderWidth / 2);
			int borderStartY = GridLocationY + (BorderWidth / 2);
			BorderStartLocation = new Point (borderStartX, borderStartY);
		}

		public void OrderRefresh (Cell[] cellsToRefresh)
		{
			Rectangle zoneToRefresh = Rectangle.Empty;

			foreach (Cell cell in cellsToRefresh) {

				int x = BorderStartLocation.X + cell.X * parent.CellSize;
				int y = BorderStartLocation.Y + cell.Y * parent.CellSize;

				Rectangle cellRect = new Rectangle (x, y, parent.CellSize, parent.CellSize);

				if (zoneToRefresh == Rectangle.Empty) {
					zoneToRefresh = cellRect;
				} else {
					if (zoneToRefresh.Contains (cellRect) == false || zoneToRefresh.IntersectsWith (cellRect) == false) {

						zoneToRefresh = Rectangle.Union (cellRect, zoneToRefresh);
					}
				}
			}

			// Trigger the refresh if necessary
			OrderRefresh (zoneToRefresh);
		}
		/// <summary>
		/// Common draw function for multiplatform rendering
		/// </summary>
		public void DrawPuzzle ()
		{
			// Initialize the drawing context
			StartDraw ();

			// Draw the grid and cells
			DrawGrid ();

			// Draw each cell 
			// ------------------------------------------------------------
			for (int x=0; x<parent.Width; x++) {
				for (int y=0; y<parent.Height; y++) {
				
					Cell cell = parent.GetCell (x, y);

					// Doesn't exists?
					if (cell == null)
						continue;

					// Get borders
					int cellStartX = BorderStartLocation.X + (x * parent.CellSize);
					int cellStartY = BorderStartLocation.Y + (y * parent.CellSize);
					Rectangle cellRect = new Rectangle (cellStartX, cellStartY, parent.CellSize, parent.CellSize);

					// Check if the cell has to be refreshed
					if (IsToRefresh (cell, cellRect) == false)
						continue;

					// Get properties
					bool hasPath = (cell.Path != null);
					bool isStartOrEnd = cell.IsPathStartOrEnd;
					bool isValid = (hasPath && cell.Path.IsValid);
					bool isLastCell = (hasPath && cell.Path.IsLastCell (cell));

					// Draw what's necessary
					if(isValid || isStartOrEnd) 
					{
						DrawCellBase (cellRect, isValid, isStartOrEnd, cell.Color);
					}

					// Draw paths
					if (hasPath) 
					{
						// Get the path!
						// For this each cell of the path draw the cell just before them
						Cell previousCell = cell.Path.PreviousCell (cell);
						if (previousCell != null) {

							// Get the direction of the previous cell
							int previousDirectionX = previousCell.X - cell.X;
							int previousDirectionY = previousCell.Y - cell.Y;

							// If x or y is set then y or x has to be 0

							// Draw an ellipse between the two cells
							// This code is brutal
							int pathStartX;
							int pathStartY;
							int pathWidth;
							int pathHeight;

							if (previousDirectionX != 0) {

								// Horizontal path
								pathWidth = parent.CellSize;
								pathHeight = parent.CellSize / 2;

								// Get the middle X of the previous cell
								pathStartX = cellStartX + (previousDirectionX * pathWidth / 2);

								// Center Y in the current cell
								pathStartY = cellStartY + ((parent.CellSize - pathHeight) / 2);

							} else {

								// Vertical path
								pathWidth = parent.CellSize / 2;
								pathHeight = parent.CellSize;

								// Center X in the current cell
								pathStartX = cellStartX + ((parent.CellSize - pathWidth) / 2);

								// Get the middle Y of the previous cell
								pathStartY = cellStartY + (previousDirectionY * pathHeight / 2);

							}

							Rectangle pathRect = new Rectangle (
								pathStartX,
								pathStartY,
								pathWidth,
								pathHeight
							);


							DrawPath (pathRect, new Point (previousDirectionX, previousDirectionY), cell.Path.Color);

							// Text!
							// -- Last cell of an incomplete path?
							if ((isLastCell == true) && (isValid == false) && (isStartOrEnd == false)) 
							{
								DrawLastCellIncompletePath (cellRect, cell.Path.Length.ToString (), cell.Path.Color);
							}
						}

					} // path

					// Text for node value at ends/Starts
					if (isStartOrEnd) 
					{
						// Draw the text
						DrawEndOrStartText (cellRect, cell.Path.ExpectedLength.ToString (), cell.Path.Color);
					}
				} // y
			} // x

			// End the drawing
			EndDraw ();
		}

		#endregion

		#region iOS Drawing

		// ------------------------------------------------------
		// ------------------------------------------------------
		// iOS drawing
		// ------------------------------------------------------
		// ------------------------------------------------------
		private CGContext context;
		private Rectangle drawRect;

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			// Save the rect
			this.drawRect = new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

			// Order the big draw
			DrawPuzzle ();
		}

		public virtual void OrderRefresh (Rectangle zoneToRefresh)
		{
			// iOS Specific
			SetNeedsDisplayInRect (zoneToRefresh);
		}

		public virtual void StartDraw ()
		{
			// Context properties
			this.context = UIGraphics.GetCurrentContext ();

			// -- Text
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.TextMatrix = CGAffineTransform.MakeScale (1f, -1f);

		}

		public virtual bool IsToRefresh (Cell cell, Rectangle cellRect)
		{
			return drawRect.IntersectsWith (cellRect) || drawRect.Contains (cellRect);
		}

		public virtual void DrawGrid ()
		{
			// Draw the borders of the grid
			// ------------------------------------------------------------
			int borderEndX = BorderStartLocation.X + (parent.CellSize * parent.Width) + (BorderWidth / 2);
			int borderEndY = BorderStartLocation.Y + (parent.CellSize * parent.Height) + (BorderWidth / 2);

			context.SetStrokeColorWithColor (UIColor.Blue.CGColor);
			context.MoveTo (BorderStartLocation.X, BorderStartLocation.Y); //start at this point
			context.SetLineWidth (BorderWidth);

			// Draw a square
			context.AddLineToPoint (borderEndX, BorderStartLocation.Y); 
			context.AddLineToPoint (borderEndX, borderEndY); 
			context.AddLineToPoint (BorderStartLocation.X, borderEndY); 
			context.AddLineToPoint (BorderStartLocation.X, BorderStartLocation.Y); 

			// Effective draw
			context.StrokePath ();

			// Draw cells 
			// ------------------------------------------------------------
			context.SetStrokeColorWithColor (UIColor.Black.CGColor);
			context.SetLineWidth (1.0f);
			context.MoveTo (BorderStartLocation.X, BorderStartLocation.X);

			for (int x=0; x<parent.Width; x++) {
				int cellBorderX = BorderStartLocation.X + x * parent.CellSize;
				context.MoveTo (cellBorderX, BorderStartLocation.X);
				context.AddLineToPoint (cellBorderX, BorderStartLocation.X + parent.Height * parent.CellSize);
			}

			for (int y=0; y<parent.Height; y++) {
				int cellBorderY = BorderStartLocation.X + y * parent.CellSize;
				context.MoveTo (BorderStartLocation.X, cellBorderY);
				context.AddLineToPoint (BorderStartLocation.X + parent.Width * parent.CellSize, cellBorderY);
			}

			context.StrokePath ();
		}

		public virtual void DrawCellBase (Rectangle rectangle, bool isValid, bool isPathEndOrStart, CellColor cellColor)
		{
			CGColor color = cellColor.UIColor.CGColor;
			context.SetFillColor (color);

			// Draw a circle of the color
			// But reduce the circle value
			int circleReductionValue = parent.CellSize / 10;

			RectangleF cellValueRect = new RectangleF (rectangle.X + circleReductionValue, rectangle.Y + circleReductionValue, parent.CellSize - 2 * circleReductionValue, parent.CellSize - 2 * circleReductionValue);
			if (isValid == false) {
				context.FillEllipseInRect (cellValueRect);
			} else {
				context.FillRect (cellValueRect);
			}
		}

		public virtual void DrawPath (Rectangle pathRect, Point direction, CellColor color)
		{
			context.SetFillColor (color.UIColor.CGColor);

			// Draw in 2 parts:
			// First a rect
			context.FillRect (pathRect);

			// Then an arc to the end
			if (direction.X < 0) {
				context.MoveTo (pathRect.Right, pathRect.Top);
				context.AddCurveToPoint (
					pathRect.Right + pathRect.Width / 3,
					pathRect.Top + pathRect.Height / 3,
					pathRect.Right + pathRect.Width / 3,
					pathRect.Top + 2 * pathRect.Height / 3,
					pathRect.Right,
					pathRect.Bottom
					);
			} else if (direction.X > 0) {
				context.MoveTo (pathRect.Left, pathRect.Top);
				context.AddCurveToPoint (
					pathRect.Left - pathRect.Width / 3,
					pathRect.Top + pathRect.Height / 3,
					pathRect.Left - pathRect.Width / 3,
					pathRect.Top + 2 * pathRect.Height / 3,
					pathRect.Left,
					pathRect.Bottom
					);
			}
			if (direction.Y < 0) {
				context.MoveTo (pathRect.Left, pathRect.Bottom);
				context.AddCurveToPoint (
					pathRect.Left + pathRect.Width / 3,
					pathRect.Bottom + pathRect.Height / 3,
					pathRect.Left + 2 * pathRect.Width / 3,
					pathRect.Bottom + pathRect.Height / 3,
					pathRect.Right,
					pathRect.Bottom
					);
			} else if (direction.Y > 0) {
				context.MoveTo (pathRect.Left, pathRect.Top);
				context.AddCurveToPoint (
					pathRect.Left + pathRect.Width / 3,
					pathRect.Top - pathRect.Height / 3,
					pathRect.Left + 2 * pathRect.Width / 3,
					pathRect.Top - pathRect.Height / 3,
					pathRect.Right,
					pathRect.Top
					);
			}

			context.FillPath ();
		}

		public virtual void DrawLastCellIncompletePath (Rectangle rect, string pathValue, CellColor color)
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

		public virtual void DrawEndOrStartText (Rectangle location, string text, CellColor color)
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

		public virtual void EndDraw ()
		{

		}

		#endregion

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

				Console.WriteLine (fingerLocation);

				Cell cell = getCellFromViewCoordinates (fingerLocation);

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
	public class GridView : Grid
	{
		// Constants
		public const int CellSizeIphone = 32;
		public const int CellSizeIpad = 48;

		public GridView (int width, int height)
			: base(width, height, AppDelegate.UserInterfaceIdiomIsPhone ? CellSizeIphone : CellSizeIpad)
		{
			// Create the view 
			View = new GridViewInternal (this, new RectangleF (0, 0, width * CellSize, height * CellSize));

			// Create the grid and cells views
			CreateGrid ();
		}

		public override void UpdateView (List<Cell> cellsToUpdate)
		{
			View.OrderRefresh (cellsToUpdate.ToArray());
		}

		internal GridViewInternal View {
			get;
			set;
		}
	}
}

