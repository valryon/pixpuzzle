using System;
using System.Linq;
using System.Collections.Generic;

namespace PixPuzzle.Data
{
	/// <summary>
	/// Path made of cell, an ordered list of cells with a start and an end
	/// </summary>
	public class Path
	{
		#region Constructor and... deconstructor!

		/// <summary>
		/// Initializes a new instance of the <see cref="PixPuzzle.Data.Path"/> class.
		/// </summary>
		/// <param name="firstCell">First cell.</param>
		/// <param name="expectedLength">Expected length.</param>
		public Path (Cell firstCell, int expectedLength)
		{
			if (firstCell == null) {
				throw new ArgumentException ();
			}
			Cells = new List<Cell> ();
			Cells.Add (firstCell);
			Color = firstCell.Color;

			ExpectedLength = expectedLength;
		}

		~Path ()
		{
			Cells.Clear ();
			Cells = null;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a cell to the path
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void AddCell (Cell cell)
		{
			Cells.Add (cell);
			cell.Path = this;
		}

		/// <summary>
		/// Removes all the cells that are after the given one
		/// </summary>
		/// <param name="cell">Cell.</param>
		public List<Cell> RemoveCellAfter (Cell cell)
		{
			int index = Cells.IndexOf (cell);

			List<Cell> cellsToRemove = new List<Cell> ();

			for (int i=index+1; i < Cells.Count; i++) {
				cellsToRemove.Add (Cells[i]);
			}

			foreach (var cellToRemove in cellsToRemove) {
				Cells.Remove (cellToRemove);

				cellToRemove.Path = null;
			}

			return cellsToRemove;
		}

		/// <summary>
		/// Delete the path, make it disappear
		/// </summary>
		public List<Cell> DeleteItself ()
		{
			List<Cell>  removedCells = new List<Cell> ();

			// Prevent 1 path to be deleted
			if (Cells.Count <= 1)
				return removedCells;

			foreach (var c in Cells) {

				removedCells.Add (c);

				if (c.IsPathStartOrEnd == false) {
					c.Path = null;
				} else {
					// Do not lose information for the start and end
					c.Path = new Path (c, ExpectedLength);
				}
			}

			Cells.Clear ();

			return removedCells;
		}

		/// <summary>
		/// Place in the chain
		/// </summary>
		/// <returns>The of.</returns>
		/// <param name="cell">Cell.</param>
		public int IndexOf (Cell cell)
		{
			return Cells.IndexOf (cell);
		}

		/// <summary>
		/// Fusion the specified other path.
		/// </summary>
		/// <param name="otherPath">Other path.</param>
		public void Fusion (Path otherPath)
		{
			// The otherPath is the loser
			// We transfer its data
			// Then we kill it
			foreach (Cell cell in otherPath.Cells) {
				if (Cells.Contains (cell) == false) {
					AddCell (cell);
				}
			}

			// Sort cells
			// Get the end and the start
			var endAndStart = Cells.Where (c => c.IsPathStartOrEnd).OrderBy (c => c.X).ThenBy (c => c.Y);

			Cells = Cells.Where(c => c.IsPathStartOrEnd == false).OrderBy (c => c.X).ThenBy (c => c.Y).ToList();
			Cells.Insert (0, endAndStart.First ());

			if (endAndStart.Last () != null) {
				Cells.Add (endAndStart.Last ());
			}

			otherPath.Cells.Clear ();
		}

		public Cell PreviousCell (Cell cell)
		{
			if (Cells.Contains (cell) == false)
				return null;

			int index = Cells.IndexOf (cell);

			if(index - 1 >= 0) {
				return Cells [index - 1];
			}
			return null;
		}

		public Cell NextCell (Cell cell)
		{
			if (Cells.Contains (cell) == false)
				return null;

			int index = Cells.IndexOf (cell);

			if(index + 1 < Cells.Count) {
				return Cells [index + 1];
			}
			return null;
		}

		/// <summary>
		/// Check if the cell is the last of the path
		/// </summary>
		/// <returns><c>true</c> if this instance is last cell the specified cell; otherwise, <c>false</c>.</returns>
		/// <param name="cell">Cell.</param>
		public bool IsLastCell (Cell cell)
		{
			if (Cells.Count == 0)
				return false;

			return Cells [Cells.Count -1] == cell;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The path, an ordered list of cells
		/// </summary>
		/// <value>The path cells.</value>
		public List<Cell> Cells { get; private set; }

		/// <summary>
		/// The color is determined by the first cell
		/// </summary>
		/// <value>The color of the path.</value>
		public CellColor Color { get; private set; }

		/// <summary>
		/// The expected length of the path, in cells.
		/// </summary>
		/// <value>The expected length.</value>
		public int ExpectedLength { get; private set; }

		/// <summary>
		/// Path first cell
		/// </summary>
		/// <value>The first cell.</value>
		public Cell FirstCell {
			get {
				if (Cells.Count == 0)
					return null;

				return Cells [0];
			}
		}

		/// <summary>
		/// The path is closed and has the expected length
		/// </summary>
		/// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
		public bool IsValid {
			get {
				return IsClosed && (ExpectedLength == Length);
			}
		}

		/// <summary>
		/// Current path length
		/// </summary>
		/// <value>The length.</value>
		public int Length {
			get {
				return Cells.Count;
			}
		}

		/// <summary>
		/// A closed path is a path where the first and last cells contains a number
		/// </summary>
		/// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
		public bool IsClosed {
			get {
				// Path is empty = obviously not closed
				if (Cells.Count == 0) {
					return false;
				}

				Cell firstCell = Cells [0];
				Cell lastCell = Cells [Cells.Count - 1];

				// Only one cell, more tricky, can be valid
				if (firstCell == lastCell) {
					return ExpectedLength == 1;
				}

				return firstCell.IsPathStartOrEnd && lastCell.IsPathStartOrEnd;
			}
		}

		#endregion

        public override string ToString()
        {
            return string.Format("{0}/{1} {2} Valid:{3} Closed:{4}", Length, ExpectedLength, Color, IsValid, IsClosed);
        }
	}
}

