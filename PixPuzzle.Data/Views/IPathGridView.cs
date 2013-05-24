using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public interface IPathGridView : IGridView<PathCell>
	{
		void DrawPath (PathCell cell,Rectangle pathRect, Point direction, CellColor color);

		void DrawLastCellIncompletePath (PathCell cell,Rectangle rect, string pathValue, CellColor color);

		void DrawCellText (PathCell cell, Rectangle location, string text, CellColor color);
	}
}

