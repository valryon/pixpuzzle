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
			Random random = new Random (182); // Predictable seed
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
				// -- We use a 4-directional algorithms
				List<Point> availableDirections = new List<Point> ()
				{
					new Point(-1,0),
					new Point(1,0),
					new Point(0,-1),
					new Point(0,1)
				};
				// -- Use a random direction for better results
				int borders = 0;
				while (availableDirections.Count > 0) {

					int index = random.Next (availableDirections.Count);

					Point p = availableDirections [index];
					availableDirections.Remove (p);

					GridCell nextCell = getCell (currentCell.X + p.X, currentCell.Y + p.Y);
					if (nextCell != null && nextCell.IsMarked == false && nextCell.Color.Equals (firstCell.Color)) {
						cellToExplore.Push (nextCell);
					}
					else {
						borders +=1;
					}
				}
				bool stopFloodFill = false;

				// If we got stuck in a corner
				if(borders == 4) 
				{
					stopFloodFill = true;
				}

				if(count >= 9) 
				{
					stopFloodFill = true;
				}

				// Stop flood fill
				if(stopFloodFill) {
					cellToExplore.Clear();
					
					// Mark as last
					lastCell = currentCell;
				}
			}

			// Complete "1" series
			if(firstCell == lastCell) 
			{
				firstCell.MarkComplete();
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

