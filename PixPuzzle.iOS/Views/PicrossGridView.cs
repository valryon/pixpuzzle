using System;
using PixPuzzle.Data;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace PixPuzzle
{
	internal class PicrossGridViewInternal : UIView, IPicrossGridView
	{
		private PicrossGridView parent;
		private CGContext context;
		private RectangleF drawRect, linesNumbersRect, colNumbersRect;

		public PicrossGridViewInternal (PicrossGridView parent, RectangleF frame) 
			: base(frame)
		{
			this.parent = parent;
		}
		#region Grid tools

		private PicrossCell getCellFromViewCoordinates (PointF viewLocation)
		{
			int x = (int)((viewLocation.X - parent.GridLocation.X) / (float)parent.CellSize);
			int y = (int)((viewLocation.Y - parent.GridLocation.Y) / (float)parent.CellSize);

			return parent.GetCell (x, y);
		}
		#endregion

		#region Events

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Touch began: find the cell under the finger and register it as a path start
			if (touches.Count == 1) {
				UITouch touch = (UITouch)touches.AnyObject;

				PointF fingerLocation = touch.LocationInView (this);

				PicrossCell cell = getCellFromViewCoordinates (fingerLocation);

				parent.ChangeCellState (cell, PicrossCellState.Filled);
			}
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Create a path under the finger following the grid
			if (touches.Count == 1) {

				UITouch touch = (UITouch)touches.AnyObject;

				PointF fingerLocation = touch.LocationInView (this);

				PicrossCell cell = getCellFromViewCoordinates (fingerLocation);

				parent.ChangeCellState (cell, PicrossCellState.Filled);
			}
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
		}
		#endregion

		#region IGridView implementation

		public void InitializeViewForDrawing ()
		{
			// Get maximum text size
			int maxLineNumbers = 0;

			foreach (PicrossSerie line in parent.Lines) {
				maxLineNumbers = Math.Max (line.Numbers.Count, maxLineNumbers);
			}

			int maxColumnNumbers = 0;

			foreach (PicrossSerie col in parent.Columns) {
				maxColumnNumbers = Math.Max (col.Numbers.Count, maxColumnNumbers);
			}

			int xMargin = maxLineNumbers * parent.CellSize;
			int yMargin = maxColumnNumbers * parent.CellSize;

			parent.GridLocation = new Point (xMargin, yMargin);

			this.Frame = new RectangleF (0, 0
			                             , (parent.CellSize * parent.Width) + parent.GridLocation.X + parent.BorderWidth + xMargin
			                             , (parent.CellSize * parent.Height) + parent.GridLocation.Y + parent.BorderWidth + yMargin
			);

			linesNumbersRect = new RectangleF (Frame.X, Frame.Y, xMargin, Frame.Height);
			colNumbersRect = new RectangleF (Frame.X, Frame.Y, Frame.Width, yMargin);

			this.BackgroundColor = UIColor.FromRGB (230, 230, 230);
		}

		public void OrderRefresh (Rectangle zoneToRefresh)
		{
			// iOS Specific
			SetNeedsDisplayInRect (zoneToRefresh);

			// Numbers too
			SetNeedsDisplayInRect (linesNumbersRect);
			SetNeedsDisplayInRect (colNumbersRect);
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			// Save the rect
			this.drawRect = new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

			// Order the big draw
			parent.DrawPuzzle ();
		}

		public void StartDraw ()
		{
			// Context properties
			this.context = UIGraphics.GetCurrentContext ();

			// -- Text
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.TextMatrix = CGAffineTransform.MakeScale (1f, -1f);
		}

		public bool IsToRefresh (PicrossCell cell, Rectangle cellRect)
		{
			return drawRect.IntersectsWith (cellRect) || drawRect.Contains (cellRect);
		}

		public void DrawGrid ()
		{
			// Draw the numbers
			// ------------------------------------------------------------

			context.SelectFont ("Helvetica Neue", 18.0f, CGTextEncoding.MacRoman);

			for (int x=0; x<parent.Lines.Length; x++) {

				PicrossSerie serie = parent.Lines [x];

				if (serie.IsValid) {
					context.SetFillColor (UIColor.Green.CGColor);
					context.SetStrokeColor(UIColor.Green.CGColor);
				} else {
					context.SetFillColor (UIColor.Black.CGColor);
					context.SetStrokeColor(UIColor.Black.CGColor);
				}

				for (int n=0; n< serie.Numbers.Count; n++) {

					// We draw numbers from the grid to the frame border
					// So we must pick them in the opposite order too
					PicrossSerieNumber number = serie.Numbers [serie.Numbers.Count - (n+1)];

					// Draw the number
					Point position = new Point (
						parent.GridLocation.X - (n * parent.CellSize),
						parent.GridLocation.Y + (x * parent.CellSize)
					);

					context.ShowTextAtPoint (position.X - 2 * parent.CellSize/3, position.Y + 2 * parent.CellSize / 3, number.Count.ToString ());
				}
			}

			for (int y=0; y<parent.Columns.Length; y++) {

				PicrossSerie serie = parent.Columns [y];

				for (int n=0; n<serie.Numbers.Count; n++) {

					// We draw numbers from the grid to the frame border
					// So we must pick them in the opposite order too
					PicrossSerieNumber number = serie.Numbers [serie.Numbers.Count - (n+1)];

					// Draw the number
					Point position = new Point (
						parent.GridLocation.X + (y * parent.CellSize),
						parent.GridLocation.Y - (n * parent.CellSize)
					);

					if (number.Count == number.CurrentCount) {
						context.SetFillColor (UIColor.Green.CGColor);
					} else {
						context.SetFillColor (UIColor.Black.CGColor);
					}

					context.ShowTextAtPoint (position.X + parent.CellSize / 2, position.Y - parent.CellSize / 2, number.Count.ToString ());
				}
			}

			// Draw cells 
			// ------------------------------------------------------------
			context.SetStrokeColorWithColor (UIColor.Black.CGColor);
			context.SetLineWidth (1.0f);
			context.MoveTo (parent.GridLocation.X, parent.GridLocation.X);

			for (int x=0; x<parent.Width; x++) {
				int cellBorderX = parent.GridLocation.X + x * parent.CellSize;
				context.MoveTo (cellBorderX, parent.GridLocation.X);
				context.AddLineToPoint (cellBorderX, parent.GridLocation.Y + parent.Height * parent.CellSize);
			}

			for (int y=0; y<parent.Height; y++) {
				int cellBorderY = parent.GridLocation.X + y * parent.CellSize;
				context.MoveTo (parent.GridLocation.X, cellBorderY);
				context.AddLineToPoint (parent.GridLocation.X + parent.Width * parent.CellSize, cellBorderY);
			}

			context.StrokePath ();
		}

		public void DrawCellBase (PicrossCell cell, Rectangle rectangle)
		{
			// DEBUG
			if (cell.ShouldBeFilled) {
				CGColor color = UIColor.White.CGColor;
				context.SetFillColor (color);
				context.FillRect (rectangle);
			}
			if (cell.State == PicrossCellState.Filled) {
				CGColor color = UIColor.Red.CGColor;
				context.SetFillColor (color);
				context.FillRect (rectangle);
			} 
			else if (cell.State == PicrossCellState.Crossed) {
				CGColor color = UIColor.Gray.CGColor;
				context.SetStrokeColor (color);

				context.MoveTo (rectangle.Left, rectangle.Top);
				context.AddLineToPoint (rectangle.Right, rectangle.Bottom);
				context.MoveTo (rectangle.Right, rectangle.Top);
				context.AddLineToPoint (rectangle.Left, rectangle.Bottom);

				context.StrokePath ();
			}
		}

		public void EndDraw ()
		{
		}
		#endregion	
	}

	public class PicrossGridView : PicrossGrid
	{
		public const int CellSizeIphone = 32;
		public const int CellSizeIpad = 48;

		public PicrossGridView (int width, int height)
				: base(width, height, AppDelegate.UserInterfaceIdiomIsPhone ? CellSizeIphone : CellSizeIpad)
		{
			PicrossGridViewInternal = new PicrossGridViewInternal (this, 
			                                                       new RectangleF (0, 0, width * CellSize, height * CellSize));

			CreateGrid (0, 0, PicrossGridViewInternal);
		}

		internal PicrossGridViewInternal PicrossGridViewInternal {
			get;
			private set;
		}
	}
}
