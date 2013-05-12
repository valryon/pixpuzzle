using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace PixPuzzle
{
	public class GameGrid : UIView
	{
		// Constants
		public const int MaximumPathLength = 9;
		public const int CellSizeIphone = 32;
		public const int CellSizeIpad = 48;
		// Events
		public event Action GridCompleted;
		// Grid
		private GridCell[][] cells;
		private int width, height;
		// Path data
		private GridCell firstPathCell, lastSelectedCell;

		public GameGrid (int imageWidth, int imageHeight)
			: base()
		{
			if (AppDelegate.UserInterfaceIdiomIsPhone) {
				CellSize = CellSizeIphone;
			} else {
				CellSize = CellSizeIpad;
			}

			// Create the view 
			Frame = new RectangleF (0, 0, imageWidth * CellSize, imageHeight * CellSize);

			// -- Gestures

			// ---- Double tap to delete
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (doubleTapEvent);
			tapGesture.NumberOfTapsRequired = 2;
			this.AddGestureRecognizer (tapGesture);

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

				// Hack
				int max = MaximumPathLength;

				if (currentCell == getCell(0,0)) {
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

		private void doubleTapEvent (UIGestureRecognizer sender)
		{
			if (sender.NumberOfTouches > 0) {

				PointF touchLocation = sender.LocationInView (this);

				GridCell cell = getCellFromViewCoordinates (touchLocation);
				removePath (cell);
			}
		}

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
				if (firstPathCell != null) {
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

					// Debug
					Console.WriteLine ("**Cell X:"+cell.X + " cell:"+cell.Y);
					Console.WriteLine ("**Path state - closed:"+isPathClosed + " isPathLastCell:"+isPathLastCell);

					if (isPathClosed == false && isPathLastCell) {
						firstPathCell = cell.Path.FirstCell;

						cell.SelectCell ();
						lastSelectedCell = cell;

						Console.WriteLine ("Starting a new path.");

						return true;
					} 
				}
			}

			Console.WriteLine ("New path not allowed here.");

			return false;
		}

		private void createPath (GridCell cell)
		{
			// The path length must be respected
			bool lengthOk = (firstPathCell.Path.Length < firstPathCell.Path.ExpectedLength);
			
			if (lengthOk == false) {
				Console.WriteLine ("The path is too long!");
			}

			// We're in the grid and we moved (not the same cell)
			if (cell != null && cell != lastSelectedCell) {

				cell.SelectCell ();

				if (cell.Path == null) {
					// The cell is available for path

					// Add the cell to the path
					if (lengthOk) {
						firstPathCell.Path.AddCell (cell);
						cell.DefinePath (firstPathCell.Path);

						Console.WriteLine ("Adding cell to the path.");
						Console.WriteLine ("Current path length: "+cell.Path.Length);
					}

				} else {

					// Already a path in the target cell

					bool sameColor = cell.Path.Color.Equals (firstPathCell.Path.Color);
					bool sameLength = (cell.Path.ExpectedLength == firstPathCell.Path.ExpectedLength);

					// -- It's a completely different path, do not override
					if (sameColor == false
						|| sameLength == false) {

						Console.WriteLine ("Cannot mix two differents path.");

						endPathCreation (false);
					} else if (cell.IsPathStartOrEnd && firstPathCell != cell) {

						// Does it cloes the path?
						if (firstPathCell.Path.Length + 1 == firstPathCell.Path.ExpectedLength) {

							// Fusion!
							Console.WriteLine ("Fusion!");
							firstPathCell.Path.Fusion (cell.Path);
							cell.DefinePath (firstPathCell.Path);

							// End the creation, the path is complete
							Console.WriteLine ("Path complete!");
							endPathCreation (true);
						} else {
							Console.WriteLine ("Path has not the right lenght!");
							endPathCreation (false);
						}
					} else if (firstPathCell.Path.Cells.Contains (cell) 
						&& Math.Abs (firstPathCell.Path.IndexOf (lastSelectedCell) - firstPathCell.Path.IndexOf (cell)) == 1) {
						// We're getting back 
						// Remove all the cells past the one we jut reached
						// The current cell will NOT be removed
						Console.WriteLine ("Removing cell after "+ cell);
						firstPathCell.Path.RemoveCellAfter (cell);
					} else {
						// I don't know what's we're doing.
						// ABANDON ALL THE WORK

						// You cannot loop so easily!
						Console.WriteLine ("I'm doing shit.");

						endPathCreation (false);
					}

				}

			} else if (cell != firstPathCell && cell != lastSelectedCell) {

				Console.WriteLine ("Invalid cell for path");

				// The cell is invalid (probably out of the grid)
				// Stop the path
				endPathCreation (false);
			}

			lastSelectedCell = cell;
		}

		private void endPathCreation (bool success)
		{
			// Check the created path
			if (firstPathCell != null && lastSelectedCell != null) {

				Console.WriteLine ("End path creation (success: "+success+")");

				// Path complete?
				if (firstPathCell.Path.IsValid) {
					foreach (GridCell cell in firstPathCell.Path.Cells) {
						cell.MarkComplete ();
					}
				}

				// Check if grid is complete
				// = if all cells are in a valid path
				bool isComplete = true;
				for (int x=0; x<width; x++) {
					for (int y=0; y<height; y++) {
						isComplete &= (cells [x] [y].Path.IsValid);
					}
				}

				if (isComplete) {
					endGrid ();
				}

				// Unselect cell
				lastSelectedCell.UnselectCell (success);
				lastSelectedCell = null;
				firstPathCell = null;
			}
		}

		private void endGrid ()
		{

			Console.WriteLine ("Grid complete!");

			if (GridCompleted != null) {
				GridCompleted ();
			}
		}

		private void removePath (GridCell cell)
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
			set; 
		}
	}
}

