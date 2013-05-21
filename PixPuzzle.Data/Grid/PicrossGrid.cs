using System;

namespace PixPuzzle.Data
{
	/// <summary>
	/// Picross-like grid.
	/// </summary>
	public class PicrossGrid : Grid<PathCell>
	{
		public PicrossGrid (int imageWidth, int imageHeight, int cellSize)
			: base(imageWidth, imageHeight, cellSize)
		{
		}

		public override void SetupGrid ()
		{
			throw new NotImplementedException ();
		}

		public override void DrawPuzzle ()
		{
			throw new NotImplementedException ();
		}

	}
}

