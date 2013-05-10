using System;
using MonoTouch.UIKit;
using System.Drawing;

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
			cells [x] [y].SetColor (color);
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
						createSerie (x, y,firstCell);
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
		private GridCell createSerie(int x,int y, GridCell firstCell) {

			firstCell.IsMarked = true;
			GridCell lastCell = firstCell;

			// Start from the first cell
			int count = 1;
			int currentX = x;
			int currentY = y;
			bool isStuck = false;

			// Try to move in another direction
			while(isStuck == false) 
			{
				// - Right
				currentX += 1;

				// Apply direction
				if ((currentX >= 0 && currentX < width) && (currentY >= 0 || currentY < height))
				{
					// Get the cell
					GridCell currentCell = cells [currentX] [currentY];

					// Already marked?
					if(currentCell.IsMarked == false && currentCell.Color.Equals(firstCell.Color)) {
						// The cell is part of the serie
						count++;
						markCell(currentCell, firstCell);

						// We may have found the last serie cell (if we're stuck)
						lastCell = currentCell; 
					}
					else {
						isStuck = true;
					}
				}
				else {
					isStuck = true;
				}
			}

			firstCell.SetValue("/"+count);
			lastCell.SetValue(count+"/");

			return lastCell;
		}

		private void markCell(GridCell cell, GridCell firstCell) {
			cell.SetColor(firstCell.Color);
			cell.IsMarked = true;
		}

		private void findAllNeighbors (int x, int y,GridCell firstCell, int count = 1)
		{
			bool left = runSerie (x - 1, y, firstCell, count);

			if (left) {
				findAllNeighbors (x - 1, y, firstCell, count + 1);
			}

			bool right = runSerie (x + 1, y, firstCell, count);
				
			if (right) {
				findAllNeighbors (x + 1, y, firstCell, count + 1);
			}

			bool up = runSerie (x, y - 1, firstCell, count);
				
			if (up) {
				findAllNeighbors (x, y - 1, firstCell, count + 1);
			}

			bool down = runSerie (x, y + 1, firstCell, count);
				
			if (down) {
				findAllNeighbors (x, y + 1, firstCell, count + 1);
			}
		}

		private bool runSerie (int x, int y, GridCell firstCell, int count)
		{

			if (x < 0 || x >= width)
				return false;
			if (y < 0 || y >= height)
				return false;

			GridCell currentCell = cells [x] [y];

			if(currentCell.IsMarked) {
				return false;
			}

			currentCell.SetValue (count.ToString ());
			currentCell.IsTextDisplayed (true);

			currentCell.IsMarked = true;

			return true;
		}
	}
}

