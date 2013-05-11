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

		// Path data
		private GridCell firstSelectedCell, lastSelectedCell;

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

		#region Grid creation

		public void SetPixelData (int x, int y, CellColor color)
		{
			// Tell the cell we now have data
			cells [x] [y].DefineBaseColor (color);
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
						/* GridCell lastCell =*/
						createSerie (firstCell);
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
				List<Point> availableDirections = new List<Point> () {
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
					} else {
						borders += 1;
					}
				}
				bool stopFloodFill = false;

				// If we got stuck in a corner
				if (borders == 4) {
					stopFloodFill = true;
				}

				if (count >= 9) {
					stopFloodFill = true;
				}

				// Stop flood fill
				if (stopFloodFill) {
					cellToExplore.Clear ();
					
					// Mark as last
					lastCell = currentCell;
				}
			}

			// Complete "1" series
			if (firstCell == lastCell) {
				firstCell.MarkComplete ();
			}

			firstCell.DefineCellAsPathStartOrEnd (count);
			lastCell.DefineCellAsPathStartOrEnd (count);

			return lastCell;
		}

		#endregion

		#region Grid tools

		private GridCell getCell (int x, int y)
		{
			if ((x >= 0 && x < width) && (y >= 0 && y < height)) {
				// Get the cell
				return cells [x] [y];
			}

			return null;
		}

		private GridCell getCellFromViewCoordinates (PointF viewLocation)
		{

			int x = (int)(viewLocation.X / (float)CellSize);
			int y = (int)(viewLocation.Y / (float)CellSize);

			return getCell (x, y);
		}

		#endregion

		#region Events

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Touch began: find the cell under the finger and register it as a path start
			if (touches.Count == 1) {
				UITouch touch = (UITouch)touches.AnyObject;

				PointF fingerLocation = touch.LocationInView (this);

				GridCell cell = getCellFromViewCoordinates (fingerLocation);

				bool pathStarted = startPathCreation (cell);

				if(pathStarted) 
				{
					this.BringSubviewToFront (cell);
				}
			}
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			// Create a path under the finger following the grid
			if (touches.Count == 1) {

				// Valid movement
				if (firstSelectedCell != null) {
					UITouch touch = (UITouch)touches.AnyObject;

					PointF fingerLocation = touch.LocationInView (this);

					GridCell cell = getCellFromViewCoordinates (fingerLocation);

					createPath (cell);
				}
			}
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			endPathCreation ();

			base.TouchesEnded (touches, evt);
		}

		#endregion

		#region Path creation behavior

		private bool startPathCreation(GridCell cell) {
			if (cell != null) {
				// Check if the cell has a valid path object.
				// If not or already closed path, do nothing
				// (you cannot take a cell without path and start messing around)
				if (cell.Path != null && cell.Path.IsClosed == false && cell.Path.IsLastCell(cell)) {
					firstSelectedCell = cell;

					firstSelectedCell.SelectCell ();

					lastSelectedCell = firstSelectedCell;

					return true;
				}
			}

			return false;
		}

		private void createPath(GridCell cell) {
			// We're in the grid and the cell is available for path
			if (cell != null && cell.Path == null) {
				// Add the cell to the path
				firstSelectedCell.Path.Cells.Add (cell);
				cell.DefinePath (firstSelectedCell.Path);

				lastSelectedCell = cell;

				Console.WriteLine ("Current path length: "+cell.Path.Length);
			}
			// Else we cancel the current path creation 
			else if(cell != firstSelectedCell && cell != lastSelectedCell){
				endPathCreation();
			}
		}

		private void endPathCreation() {
			// Touch ended, check the path
			if (firstSelectedCell != null && lastSelectedCell != null) {
				// Check if grid is complete

				// Unselect cell
				lastSelectedCell.UnselectCell ();
				lastSelectedCell = null;
				firstSelectedCell = null;
			}
		}

		#endregion
	}
}

