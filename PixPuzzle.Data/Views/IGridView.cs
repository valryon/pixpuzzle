using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	/// <summary>
	/// Common methods for grid views
	/// </summary>
	public interface IGridView
	{
		void InitializeViewForDrawing ();

		void OrderRefresh (Rectangle zoneToRefresh);

		void StartDraw ();

		bool IsToRefresh (Cell cell, Rectangle cellRect);

		void DrawGrid ();

		void DrawCellBase (Rectangle rectangle, bool isValid, bool isPathEndOrStart, CellColor cellColor);

		void DrawPath (Rectangle pathRect, Point direction, CellColor color);

		void DrawLastCellIncompletePath (Rectangle rect, string pathValue, CellColor color);

		void DrawEndOrStartText (Rectangle location, string text, CellColor color);

		void EndDraw ();
	}
}

