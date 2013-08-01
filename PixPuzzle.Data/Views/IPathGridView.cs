using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public interface IPathGridView
	{
		void InitializeViewForDrawing ();

		void OrderRefresh (Rectangle zoneToRefresh);

		void StartDraw ();

		bool IsToRefresh (PathCell cell, Rectangle cellRect);

		void DrawGrid ();

		void DrawCellBase (PathCell cell,Rectangle rectangle);

		void EndDraw ();

		void DrawPath (PathCell cell,Rectangle pathRect, Point direction, CellColor color);

		void DrawLastCellIncompletePath (PathCell cell,Rectangle rect, string pathValue, CellColor color);

		void DrawCellText (PathCell cell, Rectangle location, string text, CellColor color);
	}
}

