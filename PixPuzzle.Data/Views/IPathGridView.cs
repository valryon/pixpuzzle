using System;
using System.Drawing;

namespace PixPuzzle.Data
{
	public interface IPathGridView : IGridView<PathCell>
	{
		void DrawPath (PathCell cell,Rectangle pathRect, Point direction, CellColor color);

		void DrawLastCellIncompletePath (PathCell cell,Rectangle rect, string pathValue, CellColor color);
	}
}

