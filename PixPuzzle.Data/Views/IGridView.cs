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
	public interface IGridView<TCell>
		where TCell : Cell
	{
		void InitializeViewForDrawing ();

		void OrderRefresh (Rectangle zoneToRefresh);

		void StartDraw ();

		bool IsToRefresh (TCell cell, Rectangle cellRect);

		void DrawGrid ();

		void DrawCellBase (TCell cell,Rectangle rectangle);

		void DrawCellText (TCell cell, Rectangle location, string text, CellColor color);

		void EndDraw ();
	}
}

