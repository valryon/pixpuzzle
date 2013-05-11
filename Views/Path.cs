using System;
using System.Collections.Generic;

namespace PixPuzzle
{
	public class Path
	{
		/// <summary>
		/// The path, an ordered list of cells
		/// </summary>
		/// <value>The path cells.</value>
		public List<GridCell> Cells { get; private set; }
		/// <summary>
		/// The color is determined by the first cell
		/// </summary>
		/// <value>The color of the path.</value>
		public CellColor Color { get; private set; }

		public int ExpectedLength { get; private set; }

		public Path (GridCell firstCell, int expectedLength)
		{
			if (firstCell == null) {
				throw new ArgumentException ();
			}
			Cells = new List<GridCell> ();
			Cells.Add (firstCell);
			Color = firstCell.Color;

			ExpectedLength = expectedLength;
		}

		public void AddCell(GridCell cell) {
			Cells.Add (cell);

			foreach (var c in Cells) {

				// Update existing cells
				if(c != cell) {
					c.UpdateViewFromPath ();
				}
			}
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
			foreach (GridCell cell in otherPath.Cells) {
				if (Cells.Contains (cell) == false) {
					AddCell (cell);
				}
			}

			otherPath.Cells.Clear ();
		}
		/// <summary>
		/// Check if the cell is the last of the path
		/// </summary>
		/// <returns><c>true</c> if this instance is last cell the specified cell; otherwise, <c>false</c>.</returns>
		/// <param name="cell">Cell.</param>
		public bool IsLastCell (GridCell cell)
		{
			if (Cells.Count == 0)
				return false;

			return Cells [Cells.Count -1] == cell;
		}

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

				GridCell firstCell = Cells [0];
				GridCell lastCell = Cells [Cells.Count - 1];

				// Only one cell, more tricky, can be valid
				if (firstCell == lastCell) {
					return ExpectedLength == 1;
				}

				return firstCell.IsPathStartOrEnd && lastCell.IsPathStartOrEnd;
			}
		}

		~Path ()
		{
			Cells.Clear ();
		}
	}
}

