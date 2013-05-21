using System;
using System.Collections.Generic;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public abstract class PathGrid
	{
		public const int MaximumPathLength = 9;
		/// <summary>
		/// Occurs when grid is completed.
		/// </summary>
		public event Action GridCompleted;
		/// <summary>
		/// The grid
		/// </summary>
		protected Cell[][] Cells;
		/// <summary>
		/// Path data
		/// </summary>
		protected Cell FirstPathCell, LastSelectedCell;

		/// <summary>
		/// Initializes a new instance of the <see cref="PixPuzzle.Data.Grid"/> class.
		/// </summary>
		/// <param name="imageWidth">Image width.</param>
		/// <param name="imageHeight">Image height.</param>
		/// <param name="cellSize">Cell size.</param>
		public PathGrid (int imageWidth, int imageHeight, int cellSize)
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
		public void CreateGrid (int locationX, int locationY, IGridView view)
		{
			// Create the grid
			Cells = new Cell[Width][];

			for (int x=0; x<Width; x++) {

				Cells [x] = new Cell[Height];

				for (int y=0; y<Height; y++) {

					Cell c = new Cell (x, y);
					Cells [x] [y] = c;
				}
			}

			// Initialize the view
			if (view == null)
				throw new ArgumentException ();

			this.View = view;

			BorderWidth = 4;
			GridLocation = new Point (0, 0);

			int borderStartX = GridLocation.X + (BorderWidth / 2);
			int borderStartY = GridLocation.Y + (BorderWidth / 2);
			BorderStartLocation = new Point (borderStartX, borderStartY);

			this.View.InitializeViewForDrawing ();
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
				List<CellPoint> availableDirections = new List<CellPoint> () {
					new CellPoint(-1,0),
					new CellPoint(1,0),
					new CellPoint(0,-1),
					new CellPoint(0,1)
				};
				// -- Use a random direction for better results
				int borders = 0;
				while (availableDirections.Count > 0) {

					int index = random.Next (availableDirections.Count);

					CellPoint p = availableDirections [index];
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

		#region View

		/// <summary>
		/// Request the grid to be updated, especially some cells that may have been modified
		/// </summary>
		/// <param name="cellsToUpdate">Cells to update.</param>
		public void UpdateView (Cell[] cellsToRefresh)
		{
			Rectangle zoneToRefresh = Rectangle.Empty;

			foreach (Cell cell in cellsToRefresh) {

				int x = BorderStartLocation.X + cell.X * CellSize;
				int y = BorderStartLocation.Y + cell.Y * CellSize;

				Rectangle cellRect = new Rectangle (x, y, CellSize, CellSize);

				if (zoneToRefresh == Rectangle.Empty) {
					zoneToRefresh = cellRect;
				} else {
#if IOS
					if (zoneToRefresh.Contains (cellRect) == false || zoneToRefresh.IntersectsWith (cellRect) == false) {
#elif WINDOWS_PHONE
                    if (zoneToRefresh.Contains (cellRect) == false || zoneToRefresh.Intersects (cellRect) == false) {
#endif
						zoneToRefresh = Rectangle.Union (cellRect, zoneToRefresh);
					}
				}
			}

			// Trigger the refresh if necessary
			View.OrderRefresh (zoneToRefresh);
		}
		/// <summary>
		/// Common draw function for multiplatform rendering
		/// </summary>
		public void DrawPuzzle ()
		{
			// Initialize the drawing context
			View.StartDraw ();

			// Draw the grid and cells
			View.DrawGrid ();

			// Draw each cell 
			// ------------------------------------------------------------
			for (int x=0; x<Width; x++) {
				for (int y=0; y<Height; y++) {

					Cell cell = GetCell (x, y);

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

					// Get properties
					bool hasPath = (cell.Path != null);
					bool isStartOrEnd = cell.IsPathStartOrEnd;
					bool isValid = (hasPath && cell.Path.IsValid);
					bool isLastCell = (hasPath && cell.Path.IsLastCell (cell));

					// Draw what's necessary
					if (isValid || isStartOrEnd) {
						View.DrawCellBase (cellRect, isValid, isStartOrEnd, cell.Color);
					}

					// Draw paths
					if (hasPath) {
						// Get the path!
						// For this each cell of the path draw the cell just before them
						Cell previousCell = cell.Path.PreviousCell (cell);
						if (previousCell != null) {

							// Get the direction of the previous cell
							int previousDirectionX = previousCell.X - cell.X;
							int previousDirectionY = previousCell.Y - cell.Y;

							// If x or y is set then y or x has to be 0

							// Draw an ellipse between the two cells
							// This code is brutal
							int pathStartX;
							int pathStartY;
							int pathWidth;
							int pathHeight;

							if (previousDirectionX != 0) {

								// Horizontal path
								pathWidth = CellSize;
								pathHeight = CellSize / 2;

								// Get the middle X of the previous cell
								pathStartX = cellStartX + (previousDirectionX * pathWidth / 2);

								// Center Y in the current cell
								pathStartY = cellStartY + ((CellSize - pathHeight) / 2);

							} else {

								// Vertical path
								pathWidth = CellSize / 2;
								pathHeight = CellSize;

								// Center X in the current cell
								pathStartX = cellStartX + ((CellSize - pathWidth) / 2);

								// Get the middle Y of the previous cell
								pathStartY = cellStartY + (previousDirectionY * pathHeight / 2);

							}

							Rectangle pathRect = new Rectangle (
								pathStartX,
								pathStartY,
								pathWidth,
								pathHeight
							);


							View.DrawPath (pathRect, new Point (previousDirectionX, previousDirectionY), cell.Path.Color);

							// Text!
							// -- Last cell of an incomplete path?
							if ((isLastCell == true) && (isValid == false) && (isStartOrEnd == false)) {
								View.DrawLastCellIncompletePath (cellRect, cell.Path.Length.ToString (), cell.Path.Color);
							}
						}

					} // path

					// Text for node value at ends/Starts
					if (isStartOrEnd) {
						// Draw the text
						View.DrawEndOrStartText (cellRect, cell.Path.ExpectedLength.ToString (), cell.Path.Color);
					}
				} // y
			} // x

			// End the drawing
			View.EndDraw ();
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

				// Debug
				Console.WriteLine ("**Cell X:"+cell.X + " Y:"+cell.Y);

				if (cell.Path != null) {

					// Check if the cell has a valid path object.
					// If not or already closed path, do nothing
					// (you cannot take a cell without path and start messing around)
					bool isPathClosed = cell.Path.IsClosed;
					bool isPathLastCell = cell.Path.IsLastCell (cell);

					// Debug
					Console.WriteLine ("**Path val:"+cell.Path.ExpectedLength+" closed:"+isPathClosed + " isPathLastCell:"+isPathLastCell);

					if (isPathClosed == false && isPathLastCell) {
						FirstPathCell = cell.Path.FirstCell;

						cell.SelectCell ();
						if (LastSelectedCell != null) {
							LastSelectedCell.UnselectCell (false);
						}
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

						// Update the modified cells
						UpdateView (cell.Path.Cells.ToArray());

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
							cell.Path = FirstPathCell.Path;

							// Update the modified cells
							UpdateView (FirstPathCell.Path.Cells.ToArray());

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
						List<Cell> removedCells = FirstPathCell.Path.RemoveCellAfter (cell);

						// Update the modified cells
						// And the removed ones
						List<Cell> cellsToUpdate = new List<Cell> ();
						cellsToUpdate.AddRange (cell.Path.Cells);
						cellsToUpdate.AddRange (removedCells);

						UpdateView (cellsToUpdate.ToArray());
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

		public void EndPathCreation (bool success = false)
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
					List<Cell> removedCells = cell.Path.DeleteItself ();
					Console.WriteLine ("Deleting the path");

					UpdateView (removedCells.ToArray());
				}
			}
		}
		#endregion

		public IGridView View {
			get;
			protected set;
		}

		public int CellSize { 
			get;
			private set; 
		}

		public int Width { 
			get;
			private set; 
		}

		public int Height { 
			get;
			private set; 
		}

		public bool IsCreatingPath {
			get {
				return FirstPathCell != null;
			}
		}

		public int BorderWidth { 
			get;
			protected set; 
		}

		public Point GridLocation { 
			get;
            protected set; 
		}

		public Point BorderStartLocation { 
			get;
            protected set; 
		}
	}
}

