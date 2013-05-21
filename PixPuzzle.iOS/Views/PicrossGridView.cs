using System;
using PixPuzzle.Data;
using MonoTouch.UIKit;
using System.Drawing;

namespace PixPuzzle
{
	internal class PicrossGridViewInternal : UIView, IGridView
	{
		private PicrossGridView parent;

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
		}

		public void StartDraw ()
		{
		}

		public bool IsToRefresh (Cell cell, Rectangle cellRect)
		{
			return false;
		}

		public void DrawGrid ()
		{
		}

		public void DrawCellBase (Rectangle rectangle, bool isValid, bool isPathEndOrStart, CellColor cellColor)
		{
		}

		public void DrawPath (Rectangle pathRect, Point direction, CellColor color)
		{
		}

		public void DrawLastCellIncompletePath (Rectangle rect, string pathValue, CellColor color)
		{
		}

		public void DrawEndOrStartText (Rectangle location, string text, CellColor color)
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
