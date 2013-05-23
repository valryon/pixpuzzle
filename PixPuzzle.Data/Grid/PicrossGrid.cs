using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	/// <summary>
	/// Picross-like grid.
	/// </summary>
	public class PicrossGrid : Grid<PicrossCell, IPicrossGridView>
	{
		public PicrossGrid (int imageWidth, int imageHeight, int cellSize)
			: base(imageWidth, imageHeight, cellSize)
		{
		}
		
		public void SetFilledPixel (int x, int y)
		{
			PicrossCell cell = GetCell(x,y);

			if(cell != null) 
			{
				cell.ShoudBeFilled = true;
			}
		}

		public override void SetupGrid ()
		{
			// We should have all pixels set up here
			// So we can deduce the grid and get the lines and columns nmbers
		}

		public override void DrawPuzzle ()
		{
			View.StartDraw ();

			// Draw grid and background
			// ------------------------------------------------------------
			View.DrawGrid ();

			// Draw each cell 
			// ------------------------------------------------------------
			for (int x=0; x<Width; x++) {
				for (int y=0; y<Height; y++) {

					PicrossCell cell = GetCell (x, y);

					// Doesn't exists?
					if (cell == null)
						continue;

					// Get borders
					int cellStartX = BorderStartLocation.X + (x * CellSize);
					int cellStartY = BorderStartLocation.Y + (y * CellSize);
					Rectangle cellRect = new Rectangle (cellStartX, cellStartY, CellSize, CellSize);

					// Check if the cell has to be refreshed
					if (View.IsToRefresh (cell, cellRect) == false)
						continue;

					View.DrawCellBase (cell, cellRect);
				}
			}

			View.EndDraw ();
		}

	}
}

