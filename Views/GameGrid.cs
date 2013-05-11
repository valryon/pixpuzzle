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

			firstCell.DefineCellAsPathStartOrEnd (count);
			lastCell.DefineCellAsPathStartOrEnd (count);

			// Complete "1" series
			if (firstCell == lastCell) {
				firstCell.MarkComplete ();
			}

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

				if (pathStarted) {
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
			endPathCreation (false);

			base.TouchesEnded (touches, evt);
		}
		#endregion

		#region Path creation behavior

		private bool startPathCreation (GridCell cell)
		{
			if (cell != null) {

				if (cell.Path != null) {

					// Check if the cell has a valid path object.
					// If not or already closed path, do nothing
					// (you cannot take a cell without path and start messing around)
					bool isPathClosed = cell.Path.IsClosed;
					bool isPathLastCell = cell.Path.IsLastCell (cell);

					if (isPathClosed == false && isPathLastCell) {
						firstSelectedCell = cell;

						firstSelectedCell.SelectCell ();

						lastSelectedCell = firstSelectedCell;

						Console.WriteLine ("Starting a new path.");

						return true;
					} else {
						Console.WriteLine ("Path state - closed:"+isPathClosed + " lastCell:"+isPathLastCell);
					}
				}
			}

			Console.WriteLine ("New path not allowed here.");

			return false;
		}

		private void createPath (GridCell cell)
		{
			// THe path must be valid
			if (firstSelectedCell.Path.Length >= firstSelectedCell.Path.ExpectedLength) {

				Console.WriteLine ("The path is too long!");

				endPathCreation (false);

			} else {
				// We're in the grid and we moved (not the same cell)
				if (cell != null && cell != lastSelectedCell) {

					lastSelectedCell = cell;

					if (cell.Path == null) {
						// The cell is available for path

						// Add the cell to the path
						firstSelectedCell.Path.AddCell (cell);
						cell.DefinePath (firstSelectedCell.Path);

						Console.WriteLine ("Adding cell to the path.");
						Console.WriteLine ("Current path length: "+cell.Path.Length);

					} else {

						// Already a path in the target cell

						bool sameColor = cell.Path.Color.Equals (firstSelectedCell.Path.Color);
						bool sameLength = (cell.Path.ExpectedLength == firstSelectedCell.Path.ExpectedLength);

						// -- It's a completely different path, do not override
						if (sameColor == false
							|| sameLength == false) {

							Console.WriteLine ("Cannot mix two differents path.");

							endPathCreation (false);
						} else if (cell.IsPathStartOrEnd) {
							// We're at an end or a start

							// Does it cloes the path?
							if (firstSelectedCell.Path.Length + 1 == firstSelectedCell.Path.ExpectedLength) {
								// Fusion!
								Console.WriteLine ("Fusion!");
								firstSelectedCell.Path.Fusion (cell.Path);
								cell.DefinePath (firstSelectedCell.Path);

								// End the creation, the path is complete
								Console.WriteLine ("Path complete!");
								endPathCreation (true);
							} else {
								Console.WriteLine ("Path is too short!");
								endPathCreation (false);
							}
						} else if (firstSelectedCell.Path.Cells.Contains (cell)) {

							Console.WriteLine ("Cannot go back");

							// We're getting back and that's not allowed
							endPathCreation (false);
						} 

					}

				} else if (cell != firstSelectedCell && cell != lastSelectedCell) {

					Console.WriteLine ("Invalid cell for path");

					// The cell is invalid (probably out of the grid)
					// Stop the path
					endPathCreation (false);
				}
			}
		}

		private void endPathCreation (bool success)
		{
			// Check the created path
			if (firstSelectedCell != null && lastSelectedCell != null) {

				Console.WriteLine ("End path creation (success: "+success+")");

				// Path complete?
				if (firstSelectedCell.Path.IsValid) {
					foreach (GridCell cell in firstSelectedCell.Path.Cells) {
						cell.MarkComplete ();
					}
				}

				// Check if grid is complete

				// Unselect cell
				lastSelectedCell.UnselectCell (success);
				lastSelectedCell = null;
				firstSelectedCell = null;
			}
		}
		#endregion
	}
}

