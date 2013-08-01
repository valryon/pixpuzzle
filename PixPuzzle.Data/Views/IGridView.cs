using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public interface IGridView
	{
		void InitializeViewForDrawing ();

		void OrderRefresh (Rectangle zoneToRefresh);

		void StartDraw ();

		bool IsToRefresh (Cell cell, Rectangle cellRect);

		void DrawGrid ();

		void DrawCellBase (Cell cell,Rectangle rectangle);

		void EndDraw ();

		void DrawPath (Cell cell,Rectangle pathRect, Point direction, CellColor color);

		void DrawLastCellIncompletePath (Cell cell,Rectangle rect, string pathValue, CellColor color);

		void DrawCellText (Cell cell, Rectangle location, string text, CellColor color);
	}
}

