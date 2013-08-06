using System;
using System.Collections.Generic;

#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	/// <summary>
	/// Pathpix-like grid
	/// </summary>
	public abstract class Grid
	{
		#region Fields

		/// <summary>
		/// The grid
		/// </summary>
		protected Cell[][] Cells;
		protected Cell FirstPathCell, LastSelectedCell;
		protected int MaxPathLength;
		private Random mRandom = new Random (182);

		#endregion

		#region Constructor & Initialization

		// Predictable seed
		/// <summary>
		/// Initializes a new instance of the <see cref="PixPuzzle.Data.Grid"/> class.
		/// </summary>
		/// <param name="imageWidth">Image width.</param>
		/// <param name="imageHeight">Image height.</param>
		/// <param name="cellSize">Cell size.</param>
		public Grid (PuzzleData puzzle, int imageWidth, int imageHeight, int cellSize)
		{
			CellSize = cellSize;
			Width = imageWidth;
			Height = imageHeight;

			// Determine maximum number for the path
			// TODO Change that, not very interesting
			MaxPathLength = 9 + ((Math.Max (imageWidth, imageHeight) / 32) * 2);
		}

		/// <summary>
		/// Create a grid and register view data
		/// </summary>
		public void CreateGrid (int locationX, int locationY, IGridView view)
		{
			Logger.I ("Creating the " + Width + "x" + Height + " grid...");

			// Calculate border size
			// TODO This sucks
			BorderWidth = 4;
			int borderStartX = GridLocation.X + (BorderWidth / 2);
			int borderStartY = GridLocation.Y + (BorderWidth / 2);
			BorderStartLocation = new Point (borderStartX, borderStartY);
			GridLocation = BorderStartLocation;

			// Create the grid
			Cells = new Cell[Width][];

			for (int x=0; x<Width; x++) {

				Cells [x] = new Cell[Height];

				for (int y=0; y<Height; y++) {

					Cell c = new Cell ();
					c.X = x;
					c.Y = y;
					Cells [x] [y] = c;

					c.Rect = new Rectangle (
						GridLocation.X + c.X * CellSize, 
						GridLocation.Y + c.Y * CellSize, 
						CellSize, CellSize);
				}
			}

			// Initialize the view
			if (view == null) {
				throw new ArgumentException ();
			}

			this.View = view;
		}

		/// <summary>
		/// Prepare the grid from image data
		/// </summary>
		public void SetupGrid (CellColor[][] pixels)
		{
			Logger.I ("Setup the grid using image pixels data...");

			// Fill cells
			for (int x=0; x<pixels.Length; x++) {
				for (int y=0; y<pixels[x].Length; y++) {

					// Get the pixel color
					CellColor c = pixels [x] [y];

					// Transparent becomes white
					if (c.A < 0.2f) {
						c = new CellColor () {
							A = 1f,
							R = 1f, 
							G = 1f, 
							B = 1f // White
						};
					}

					Cells [x] [y].Color = c;
				}	
			}

			// Look at each cell and create series
			for (int x=0; x<Width; x++) {
				for (int y=0; y<Height; y++) {

					var cell = Cells [x] [y];

					if (cell.IsMarked == false) {

						// The cell become the first of a serie
						Cell firstCell = cell;

						// Find its unmarked neighbors 
						InitializePath (firstCell);
					}
				}
			}

			this.View.InitializeViewForDrawing ();
		}

		/// <summary>
		/// Create a path from a given cell.
		/// </summary>
		/// <returns>The last cell.</returns>
		/// <param name="firstCell">First cell.</param>
		private Cell InitializePath (Cell firstCell)
		{
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

					int index = mRandom.Next (availableDirections.Count);

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

				// Limit maximum path lengh randomly
				int max = mRandom.Next (2, MaxPathLength);

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

		#region Methods

		#region Render

		public void SetFilledCells (bool isFilled)
		{
			ShouldDisplayFilledCells = isFilled;

			// Order a complete refresh
			RefreshZone (new Rectangle (
				GridLocation.X,
				GridLocation.Y,
				Width * CellSize,
				Height * CellSize
			));
		}

		/// <summary>
		/// Request the grid to be updated, especially some cells that may have been modified
		/// </summary>
		public virtual void UpdateView (Cell[] cellsToRefresh)
		{
			Rectangle zoneToRefresh = Rectangle.Empty;

			// Get a complete rectangle of cells
			foreach (Cell cell in cellsToRefresh) {

				int x = GridLocation.X + cell.X * CellSize;
				int y = GridLocation.Y + cell.Y * CellSize;

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
			} // foreach

			RefreshZone (zoneToRefresh);
		}

		void RefreshZone (Rectangle zoneToRefresh)
		{
			// Find cells to refresh in this rect
			int startX = zoneToRefresh.X / CellSize;
			int startY = zoneToRefresh.Y / CellSize;
			for (int x = startX; x < startX + (zoneToRefresh.Width / CellSize); x++) {
				for (int y = startY; y < startY + (zoneToRefresh.Height / CellSize); y++) {
					Cell c = GetCell (x, y);
					if (c != null) {
						c.IsToDraw = true;
					}
				}
			}
			// Trigger the refresh 
			if (zoneToRefresh != Rectangle.Empty) {
				View.Refresh (zoneToRefresh);
			}
		}

		/// <summary>
		/// Puzzle draw global routine
		/// </summary>
		public void DrawPuzzle ()
		{
			// Initialize the drawing context
			View.StartDraw ();

			// Get cells to draw
			List<Cell> cellsToDraw = new List<Cell> ();

			for (int x=0; x<Width; x++) {
				for (int y=0; y<Height; y++) {

					Cell cell = GetCell (x, y);

					// Doesn't exists?
					if (cell != null) {

						// Check if the cell is on the refresh short-list
						if (cell.IsToDraw) {
							cellsToDraw.Add (cell);
							cell.IsToDraw = false;
						}

					}
				}
			}

			// Layer 0 : Background
			// ================================================
			View.DrawGrid ();

			// Layer 1 : Cells base
			// ================================================
			foreach (var cell in cellsToDraw) {

				// Draw what's necessary
				if ((cell.Path != null) || (cell.IsPathStartOrEnd)) {
					View.DrawCellBase (cell);
				}
			}

			// Layer 2 : Paths
			// ================================================
			foreach (var cell in cellsToDraw) {
				// Preview mode is simplier
				if (ShouldDisplayFilledCells == false) {

					// Draw paths
					if (cell.Path != null) {

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
								pathStartX = cell.Rect.X + (previousDirectionX * pathWidth / 2);

								// Center Y in the current cell
								pathStartY = cell.Rect.Y + ((CellSize - pathHeight) / 2);

							} 
							else {

								// Vertical path
								pathWidth = CellSize / 2;
								pathHeight = CellSize;

								// Center X in the current cell
								pathStartX = cell.Rect.X + ((CellSize - pathWidth) / 2);

								// Get the middle Y of the previous cell
								pathStartY = cell.Rect.Y + (previousDirectionY * pathHeight / 2);
							}

							Rectangle pathRect = new Rectangle (
								pathStartX,
								pathStartY,
								pathWidth,
								pathHeight
							);


							View.DrawPath (cell, pathRect, new Point (previousDirectionX, previousDirectionY), cell.Path.Color);

							// -- Last cell of an incomplete path?
							if ((cell.Path != null) && (cell.Path.IsLastCell (cell)) && (cell.Path.IsValid == false) && (cell.IsPathStartOrEnd == false)) {
								View.DrawLastCellIncompletePath (cell, cell.Rect, cell.Path.Length.ToString (), cell.Path.Color);
							}
						}

					} // path
				}
			}
					
			// Layer 3 : Texts
			// ================================================
			foreach (var cell in cellsToDraw) {

				// Text for node value at ends/Starts
				if (cell.IsPathStartOrEnd) {
					// Draw the text
					View.DrawCellText (cell, cell.Rect, cell.Path.ExpectedLength.ToString (), cell.Path.Color);
				}
			}

			// End the drawing
			View.EndDraw ();
		}

		#endregion

		#region Path creation behavior

		/// <summary>
		/// Starts a new path creation.
		/// </summary>
		/// <returns><c>true</c>, if a path was started, <c>false</c> otherwise.</returns>
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

					if (isPathClosed == false && isPathLastCell) {
						FirstPathCell = cell.Path.FirstCell;

						LastSelectedCell = cell;

						return true;
					} 
				}
			}

			return false;
		}

		/// <summary>
		/// Create a path between the current known path and the given cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void CreatePath (Cell cell)
		{
			bool cancelMove = false;
			string cancelReason = string.Empty;

			// The path length must be respected
			bool lengthOk = (FirstPathCell.Path.Length < FirstPathCell.Path.ExpectedLength);

			if (lengthOk == false) {
				cancelMove = true;
				cancelReason = "The path is too long!";
			} else {
				// We're in the grid and we moved (not the same cell)
				if (cell != null && cell != LastSelectedCell) {

					// Make sure we are one cell away from the previous one
					int x = cell.X - LastSelectedCell.X;
					int y = cell.Y - LastSelectedCell.Y;

					if (Math.Abs (x) > 1 || Math.Abs (y) > 1) {
						cancelMove = true;
						cancelReason = "Cannot create a path that is not in a cell next the to the first one.";
					} else {
						if (cell.Path == null) {
							// The cell is available for path

							// Add the cell to the currently edited path
							FirstPathCell.Path.AddCell (cell);
							cell.Path = FirstPathCell.Path;

							// Update the modified cells
							UpdateView (cell.Path.Cells.ToArray ());

						} else {

							// Already a path in the target cell
							bool sameColor = cell.Path.Color.Equals (FirstPathCell.Path.Color);
							bool sameLength = (cell.Path.ExpectedLength == FirstPathCell.Path.ExpectedLength);

							// -- It's a completely different path, do not override
							if (sameColor == false || sameLength == false) {
								cancelMove = true;
								cancelReason = "Cannot mix two differents path.";
							} else if (sameColor && sameLength && FirstPathCell != cell) {

								// Fusion between two paths parts
								Logger.I ("Fusion!");
								FirstPathCell.Path.Fusion (cell.Path);
								cell.Path = FirstPathCell.Path;

								// Update the modified cells
								UpdateView (FirstPathCell.Path.Cells.ToArray ());

								// Are we ending the path?
								if (FirstPathCell.Path.IsValid) {
									// End the creation, the path is complete
									Logger.I ("Path complete!");
									EndPathCreation (true);
								}
								
							} else if (FirstPathCell.Path.Cells.Contains (cell) 
								&& Math.Abs (FirstPathCell.Path.IndexOf (LastSelectedCell) - FirstPathCell.Path.IndexOf (cell)) == 1) {
								
								// We're getting back 
								// Remove all the cells past the one we jut reached
								// The current cell will NOT be removed
								List<Cell> removedCells = FirstPathCell.Path.RemoveCellAfter (cell);

								// Update the modified cells
								// And the removed ones
								List<Cell> cellsToUpdate = new List<Cell> ();
								cellsToUpdate.AddRange (cell.Path.Cells);
								cellsToUpdate.AddRange (removedCells);

								UpdateView (cellsToUpdate.ToArray ());
							} else {
								// I don't know what's we're doing.
								// ABANDON ALL THE WORK

								// You cannot loop so easily!
								cancelMove = true;
								cancelReason = "Unreachable code... reached. Time to panic! ";
							}
						}
					}
				} else if (cell != FirstPathCell && cell != LastSelectedCell) {
					// The cell is invalid (probably out of the grid)
					// Stop the path
					cancelMove = true;
					cancelReason = "Invalid cell for path";
				}
			}

			if (cancelMove) {
				Logger.I (cancelReason);
				EndPathCreation (false);
			}
		
			LastSelectedCell = cell;
		}

		/// <summary>
		/// Ends the current path creation
		/// </summary>
		/// <param name="success">If set to <c>true</c> success.</param>
		public void EndPathCreation (bool success = false)
		{
			// Make sure we're doing path work
			if (FirstPathCell != null && LastSelectedCell != null) {
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

				// Forget cells
				LastSelectedCell = null;
				FirstPathCell = null;
			}
		}

		/// <summary>
		/// The grid is completed
		/// </summary>
		protected void EndGrid ()
		{
			Logger.I ("Grid complete!");

			if (GridCompleted != null) {
				GridCompleted ();
			}
		}

		/// <summary>
		/// Removes the path linked to the given cell
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void RemovePath (Cell cell)
		{
			if (cell != null) {
				if (cell.Path != null) {
					List<Cell> removedCells = cell.Path.DeleteItself ();
					UpdateView (removedCells.ToArray ());
				}
			}
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

		#endregion

		#region Events

		/// <summary>
		/// Occurs when grid is completed.
		/// </summary>
		public event Action GridCompleted;

		#endregion

		#region Properties

		/// <summary>
		/// View instance through a cross platform interface
		/// </summary>
		/// <value>The view.</value>
		public IGridView View {
			get;
			protected set;
		}

		public int CellSize { 
			get;
			protected set; 
		}

		public int Width { 
			get;
			private set; 
		}

		public int Height { 
			get;
			private set; 
		}

		public int BorderWidth { 
			get;
			protected set; 
		}

		public Point GridLocation { 
			get;
			set; 
		}

		public Point BorderStartLocation { 
			get;
			protected set; 
		}

		public bool IsCreatingPath {
			get {
				return FirstPathCell != null;
			}
		}

		public bool ShouldDisplayFilledCells
		{ 
			get;
			private set; 
		}

		#endregion
	}
}

