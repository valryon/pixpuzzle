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

		public Path (GridCell firstCell)
		{
			if (firstCell == null) {
				throw new ArgumentException ();
			}
			Cells = new List<GridCell> ();
			Cells.Add (firstCell);
			Color = firstCell.Color;
		}

		/// <summary>
		/// Fusion the specified other path.
		/// </summary>
		/// <param name="otherPath">Other path.</param>
		public void Fusion(Path otherPath) {

		}

		/// <summary>
		/// Check if the cell is the last of the path
		/// </summary>
		/// <returns><c>true</c> if this instance is last cell the specified cell; otherwise, <c>false</c>.</returns>
		/// <param name="cell">Cell.</param>
		public bool IsLastCell (GridCell cell)
		{
			return Cells [Cells.Count -1] == cell;
		}

		public bool IsValid
		{
			get {
				return false;
			}
		}

		/// <summary>
		/// Current path length
		/// </summary>
		/// <value>The length.</value>
		public int Length
		{
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
				// TODO First and last cell
				return false;
			}
		}

		~Path() {
			Cells.Clear ();
		}
	}
}

