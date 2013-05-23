using System;
using PixPuzzle.Data;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace PixPuzzle
{
	internal class PicrossGridViewInternal : UIView, IGridView<PicrossCell>
	{
		private PicrossGridView parent;
		private CGContext context;
		private Rectangle drawRect;

		public PicrossGridViewInternal (PicrossGridView parent, RectangleF frame) 
			: base(frame)
		{
			this.parent = parent;
		}

		#region IGridView implementation

		public void InitializeViewForDrawing ()
		{
			this.Frame = new RectangleF (parent.GridLocation.X, parent.GridLocation.Y
			                             , (parent.CellSize * parent.Width) + parent.GridLocation.X + parent.BorderWidth
			                             , (parent.CellSize * parent.Height) + parent.GridLocation.Y + parent.BorderWidth
			                             );
		}

		public void OrderRefresh (Rectangle zoneToRefresh)
		{
			// iOS Specific
			SetNeedsDisplayInRect (zoneToRefresh);
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
			return true;
		}

		public void DrawGrid ()
		{
			Point borderStartLocation = parent.BorderStartLocation;
			int borderWidth = parent.BorderWidth;

			// Draw the borders of the grid
			// ------------------------------------------------------------
			int borderEndX = borderStartLocation.X + (parent.CellSize * parent.Width) + (borderWidth / 2);
			int borderEndY = borderStartLocation.Y + (parent.CellSize * parent.Height) + (borderWidth / 2);

			context.SetStrokeColorWithColor (UIColor.Blue.CGColor);
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

		public void DrawCellBase (PicrossCell cell, Rectangle rectangle)
		{
		}

		public void DrawCellText (PicrossCell cell,Rectangle location, string text, CellColor color)
		{
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
			PicrossGridViewInternal = new PicrossGridViewInternal(this, 
			                                                      new RectangleF (0, 0, width * CellSize, height * CellSize));

			CreateGrid (0,0, PicrossGridViewInternal);
		}

		internal PicrossGridViewInternal PicrossGridViewInternal {
			get;
			private set;
		}
	}
}
