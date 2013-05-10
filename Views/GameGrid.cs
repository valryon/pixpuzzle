using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace PixPuzzle
{
	public class GameGrid : UIView
	{
		public const int CellSize = 32;
		private GridCell[][] cells;
		private int width, height;

		public GameGrid (int imageWidth, int imageHeight)
			: base(new RectangleF(0,0, imageWidth * CellSize, imageHeight * CellSize))
		{
			// Create the grid
			cells = new GridCell[imageWidth][];
			width = imageWidth;
			height = imageHeight;

			for (int x=0; x<imageWidth; x++) {
				cells [x] = new GridCell[imageHeight];

				for (int y=0; y<imageHeight; y++) {

					cells [x] [y] = new GridCell (
						x, y,
						new RectangleF (x * CellSize, y * CellSize, CellSize, CellSize)
					);

					cells [x] [y].BackgroundColor = UIColor.White;

					AddSubview (cells [x] [y]);
				}
			}
		}

		public void SetPixelData (int x, int y, CellColor color)
		{
			// Tell the cell we now have data
			cells [x] [y].Color = color;
		}

		/// <summary>
		/// Prepare the grid
		/// </summary>
		public void SetupGrid ()
		{
			// Look at each cell and create series
			for (int x=0; x<width; x++) {
				for (int y=0; y<height; y++) {

					var cell = cells [x] [y];

					if (cell.IsMarked == false) {

						// The cell become the first of a serie
						GridCell firstCell = cell;

						// Find its unmarked neighbors and the last cell of the serie
						GridCell lastCell = createSerie (firstCell);

						firstCell.IsTextDisplayed = true;
						lastCell.IsTextDisplayed = true;
					}
				}
			}
		}

		/// <summary>
		/// Create a serie from a given cell.
		/// </summary>
		/// <returns>The last cell.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="firstCell">First cell.</param>
		private GridCell createSerie (GridCell firstCell)
		{
			GridCell lastCell = firstCell;

			// Start from the first cell
			int count = 0;

			Stack<GridCell> cellToExplore = new Stack<GridCell> ();
			cellToExplore.Push (firstCell);

			// Try to move in another direction
			// Flood fill algorithm
			while (cellToExplore.Count > 0) {

				// Get the first cell to explore
				GridCell currentCell = cellToExplore.Pop ();
				count++;

				// Mark this cell
				currentCell.IsMarked = true;

				// Get the neighbors
				int borders = 0;
				GridCell leftCell = getCell (currentCell.X - 1, currentCell.Y);
				if (leftCell != null && leftCell.IsMarked == false && leftCell.Color.Equals (firstCell.Color)) {
					cellToExplore.Push (leftCell);
				}
				else {
					borders +=1;
				}
				GridCell rightCell = getCell (currentCell.X + 1, currentCell.Y);
				if (rightCell != null && rightCell.IsMarked == false && rightCell.Color.Equals (firstCell.Color)) {
					cellToExplore.Push (rightCell);
				}
				else {
					borders +=1;
				}
				GridCell upCell = getCell (currentCell.X, currentCell.Y - 1);
				if (upCell != null && upCell.IsMarked == false && upCell.Color.Equals (firstCell.Color)) {
					cellToExplore.Push (upCell);
				}
				else {
					borders +=1;
				}
				GridCell downCell = getCell (currentCell.X, currentCell.Y + 1);
				if (downCell != null && downCell.IsMarked == false && downCell.Color.Equals (firstCell.Color)) {
					cellToExplore.Push (downCell);
				}
				else {
					borders +=1;
				}

				if(borders == 4) 
				{
					lastCell = currentCell;
				}
			}

			firstCell.SetCount(count);
			lastCell.SetCount(count);

			return lastCell;
		}

		private GridCell getCell (int x, int y)
		{
			if ((x >= 0 && x < width) && (y >= 0 && y < height)) {
				// Get the cell
				return cells [x] [y];
			}

			return null;
		}

	}
}

