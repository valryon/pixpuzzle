using System;
using System.Collections.Generic;

namespace PixPuzzle.Data
{
	public abstract class Grid<TCell> 
		where TCell : Cell
	{
		// Constants
		public const int MaximumPathLength = 9;

		// Events
		public event Action GridCompleted;

		// Grid
		protected TCell[][] Cells;
		protected int Width, Height;

		// Path data
		protected Cell FirstPathCell, LastSelectedCell;

		/// <summary>
		/// Initializes a new instance of the <see cref="PixPuzzle.Data.Grid"/> class.
		/// </summary>
		/// <param name="imageWidth">Image width.</param>
		/// <param name="imageHeight">Image height.</param>
		/// <param name="cellSize">Cell size.</param>
		public Grid (int imageWidth, int imageHeight, int cellSize)
				: base()
		{
			CellSize = cellSize;
			Width = imageWidth;
			Height = imageHeight;
		}

		#region Grid creation

		/// <summary>
		/// Create a grid and initialize with default values
		/// </summary>
		/// <param name="createCell">Create cell.</param>
		public void CreateGrid(Func<int,int, TCell> createCell) {

			// Create the grid
			Cells = new TCell[Width][];

			for (int x=0; x<Width; x++) {

				Cells [x] = new TCell[Height];

				for (int y=0; y<Height; y++) {

					TCell c = createCell(x, y);
					Cells [x] [y] = c;
				}
			}
		}

		/// <summary>
		/// Define what's in the given cell
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		public void SetPixelData (int x, int y, CellColor color)
		{
			// Tell the cell we now have data
			Cells [x] [y].DefineBaseColor (color);
		}
			/// <summary>
			/// Prepare the grid
			/// </summary>
		public void SetupGrid ()
		{
			// Look at each cell and create series
			for (int x=0; x<Width; x++) {
				for (int y=0; y<Height; y++) {

					var cell = Cells [x] [y];

					if (cell.IsMarked == false) {

						// The cell become the first of a serie
						Cell firstCell = cell;

						// Find its unmarked neighbors and the last cell of the serie
						/* Cell lastCell =*/
						createPath (firstCell);
					}
				}
			}
		}
			/// <summary>
			/// Create a path from a given cell.
			/// </summary>
			/// <returns>The last cell.</returns>
			/// <param name="x">The x coordinate.</param>
			/// <param name="y">The y coordinate.</param>
			/// <param name="firstCell">First cell.</param>
		private Cell createPath (Cell firstCell)
		{
			Random random = new Random (182); // Predictable seed
			Cell lastCell = firstCell;

			// Start from the first cell
			int count = 0;

			Stack<Cell> cellToExplore = new Stack<Cell> ();
			cellToExplore.Push (firstCell);

			// Try to move in another direction
			// Flood fill algorithm
			while (cellToExplore.Count > 0) {

				// Get the first cell to explore
				Cell currentCell = cellToExplore.Pop ();
				count++;

				// Mark this cell
				currentCell.IsMarked = true;

				// Get the neighbors
				// -- We use a 4-directional algorithms
                List<CellPoint> availableDirections = new List<CellPoint>() {
					new CellPoint(-1,0),
					new CellPoint(1,0),
					new CellPoint(0,-1),
					new CellPoint(0,1)
				};
				// -- Use a random direction for better results
				int borders = 0;
				while (availableDirections.Count > 0) {

					int index = random.Next (availableDirections.Count);

                    CellPoint p = availableDirections[index];
					availableDirections.Remove (p);

					Cell nextCell = GetCell (currentCell.X + p.X, currentCell.Y + p.Y);
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

				// Hack
				int max = MaximumPathLength;

				if (currentCell == GetCell (0, 0)) {
					max += 1;
				}

				if (count >= max) {
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

			return lastCell;
		}
			#endregion

			#region Grid tools

		/// <summary>
		/// Get the cell from grid indices
		/// </summary>
		/// <returns>The cell.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Cell GetCell (int x, int y)
		{
			if ((x >= 0 && x < Width) && (y >= 0 && y < Height)) {
				// Get the cell
				return Cells [x] [y];
			}

			return null;
		}

			#endregion

			#region Path creation behavior

		/// <summary>
		/// Starts the path creation.
		/// </summary>
		/// <returns><c>true</c>, if path creation was started, <c>false</c> otherwise.</returns>
		/// <param name="cell">Cell.</param>
		public bool StartPathCreation (Cell cell)
		{
			if (cell != null) {

				if (cell.Path != null) {

					// Check if the cell has a valid path object.
					// If not or already closed path, do nothing
					// (you cannot take a cell without path and start messing around)
					bool isPathClosed = cell.Path.IsClosed;
					bool isPathLastCell = cell.Path.IsLastCell (cell);

					// Debug
					Console.WriteLine ("**Cell X:"+cell.X + " cell:"+cell.Y);
					Console.WriteLine ("**Path state - closed:"+isPathClosed + " isPathLastCell:"+isPathLastCell);

					if (isPathClosed == false && isPathLastCell) {
						FirstPathCell = cell.Path.FirstCell;

						cell.SelectCell ();
						LastSelectedCell = cell;

						Console.WriteLine ("Starting a new path.");

						return true;
					} 
				}
			}

			Console.WriteLine ("New path not allowed here.");

			return false;
		}

		/// <summary>
		/// Creates the path.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void CreatePath (Cell cell)
		{
			// The path length must be respected
			bool lengthOk = (FirstPathCell.Path.Length < FirstPathCell.Path.ExpectedLength);

			if (lengthOk == false) {
				Console.WriteLine ("The path is too long!");
			}

			// We're in the grid and we moved (not the same cell)
			if (cell != null && cell != LastSelectedCell) {

				cell.SelectCell ();

				if (cell.Path == null) {
					// The cell is available for path

					// Add the cell to the path
					if (lengthOk) {
						FirstPathCell.Path.AddCell (cell);
						cell.Path = FirstPathCell.Path;

						cell.Path.UpdateCells ();

						Console.WriteLine ("Adding cell to the path.");
						Console.WriteLine ("Current path length: "+cell.Path.Length);
					}

				} else {

					// Already a path in the target cell

					bool sameColor = cell.Path.Color.Equals (FirstPathCell.Path.Color);
					bool sameLength = (cell.Path.ExpectedLength == FirstPathCell.Path.ExpectedLength);

					// -- It's a completely different path, do not override
					if (sameColor == false
						|| sameLength == false) {

						Console.WriteLine ("Cannot mix two differents path.");

						EndPathCreation (false);
					} else if (cell.IsPathStartOrEnd && FirstPathCell != cell) {

						// Does it cloes the path?
						if (FirstPathCell.Path.Length + 1 == FirstPathCell.Path.ExpectedLength) {

							// Fusion!
							Console.WriteLine ("Fusion!");
							FirstPathCell.Path.Fusion (cell.Path);
							cell.Path  = FirstPathCell.Path;

							cell.Path.UpdateCells();

							// End the creation, the path is complete
							Console.WriteLine ("Path complete!");
							EndPathCreation (true);
						} else {
							Console.WriteLine ("Path has not the right lenght!");
							EndPathCreation (false);
						}
					} else if (FirstPathCell.Path.Cells.Contains (cell) 
						&& Math.Abs (FirstPathCell.Path.IndexOf (LastSelectedCell) - FirstPathCell.Path.IndexOf (cell)) == 1) {
						// We're getting back 
						// Remove all the cells past the one we jut reached
						// The current cell will NOT be removed
						Console.WriteLine ("Removing cell after "+ cell);
						FirstPathCell.Path.RemoveCellAfter (cell);
					} else {
						// I don't know what's we're doing.
						// ABANDON ALL THE WORK

						// You cannot loop so easily!
						Console.WriteLine ("I'm doing shit.");

						EndPathCreation (false);
					}

				}

			} else if (cell != FirstPathCell && cell != LastSelectedCell) {

				Console.WriteLine ("Invalid cell for path");

				// The cell is invalid (probably out of the grid)
				// Stop the path
				EndPathCreation (false);
			}

			LastSelectedCell = cell;
		}

		public void EndPathCreation (bool success)
		{
			// Check the created path
			if (FirstPathCell != null && LastSelectedCell != null) {

				Console.WriteLine ("End path creation (success: "+success+")");

				// Check if grid is complete
				// = if all cells are in a valid path
				bool isComplete = true;
				for (int x=0; x<Width; x++) {
					for (int y=0; y<Height; y++) {
						if (Cells [x] [y].Path != null) {
							isComplete &= (Cells [x] [y].Path.IsValid);
						} else {
							// Cell linked to no path, the grid is obvisouly not complete
							isComplete = false;
							break;
						}
					}
				}

				if (isComplete) {
					EndGrid ();
				}

				// Unselect cell
				LastSelectedCell.UnselectCell (success);
				LastSelectedCell = null;
				FirstPathCell = null;
			}
		}

		protected void EndGrid ()
		{
			Console.WriteLine ("Grid complete!");

			if (GridCompleted != null) {
				GridCompleted ();
			}
		}

		public void RemovePath (Cell cell)
		{
			if (cell != null) {
				if (cell.Path != null) {
					cell.Path.DeleteItself ();
					Console.WriteLine ("Deleting the path");
				}
			}
		}
		#endregion

		public int CellSize { 
			get;
			private set; 
		}

		public bool IsCreatingPath
		{
			get {
				return FirstPathCell != null;
			}
		}
	}
}

