using System;
using System.Drawing;

namespace PixPuzzle.Data
{
	public class Cell 
	{
		/// <summary>
		/// Request cell draw update
		/// </summary>
		public event Action<Cell> DrawCell;

		public Cell (int x, int y)
		{
			X = x;
			Y = y;

			// Default values
			DefinePath (null);
			IsPathStartOrEnd = false;
		}

		/// <summary>
		/// Sets the color for this cell from the pixel data of the image
		/// </summary>
		/// <param name="color">Color.</param>
		public void DefineBaseColor (CellColor color)
		{
			Color = color;
		}

		/// <summary>
		/// Sets the number to display. It also means that the cell is a path start or end.
		/// </summary>
		/// <param name="val">Value.</param>
		public void DefineCellAsPathStartOrEnd (int pathLength)
		{
			IsPathStartOrEnd = true;

			// The cell is the beginning or the end of a path
			DefinePath (new Path(this, pathLength));
		}

		#region Update cell

		/// <summary>
		/// Cell has been selected (touched)
		/// </summary>
		public void SelectCell ()
		{
		}

		/// <summary>
		/// Touch released
		/// </summary>
		public void UnselectCell (bool success)
		{
		}

		/// <summary>
		/// Define the path where the cell is included
		/// </summary>
		/// <param name="p">P.</param>
		public void DefinePath (Path p)
		{
			Path = p;
		}

		/// <summary>
		/// Mark the cell as being in a complete path
		/// </summary>
		public void MarkComplete ()
		{
			IsComplete = true;
		}

		/// <summary>
		/// The cell isn't in a valid path anymore
		/// </summary>
		public void UnmarkComplete ()
		{
			IsComplete = false;
		}

		public void UpdateView() {

		}

		#endregion

		#region Properties

		/// <summary>
		/// Tells if we are on a cell that is the start or the end of a complete path
		/// </summary>
		/// <value><c>true</c> if this instance is path start or end; otherwise, <c>false</c>.</value>
		public bool IsPathStartOrEnd {
			get;
			private set;
		}

		public bool IsComplete {
			get;
			private set;
		}

		/// <summary>
		/// Cell path
		/// </summary>
		/// <value>The path.</value>
		public Path Path {
			get;
			private set;
		}
		/// <summary>
		/// Location (X)
		/// </summary>
		/// <value>The x.</value>
		public int X {
			get;
			private set;
		}
		/// <summary>
		/// Location (Y)
		/// </summary>
		/// <value>The y.</value>
		public int Y {
			get;
			private set;
		}
		/// <summary>
		/// Set the right color form the image
		/// </summary>
		/// <value>The color.</value>
		public CellColor Color {
			get;
			private set;
		}
		/// <summary>
		/// Cells has been marked by grid creator
		/// </summary>
		/// <value><c>true</c> if this instance is marked; otherwise, <c>false</c>.</value>
		public bool IsMarked {
			get;
			set;
		}

		#endregion
	}
}

