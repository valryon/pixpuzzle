using System;

namespace PixPuzzle.Data
{
	/// <summary>
	/// One cell of the path puzzle
	/// </summary>
	public abstract class Cell 
	{
		public Cell (int x, int y)
		{
			X = x;
			Y = y;

			// Default values
			Path = null;
			IsPathStartOrEnd = false;
		}

		/// <summary>
		/// Sets the color for this cell from the pixel data of the image
		/// </summary>
		/// <param name="color">Color.</param>
		public void DefineBaseColor (CellColor color)
		{
			Color = color;

			UpdateView ();
		}

		/// <summary>
		/// Sets the number to display. It also means that the cell is a path start or end.
		/// </summary>
		/// <param name="val">Value.</param>
		public void DefineCellAsPathStartOrEnd (int pathLength)
		{
			IsPathStartOrEnd = true;

			// The cell is the beginning or the end of a path
			Path = new Path(this, pathLength);

			UpdateView ();
		}

		#region View

		/// <summary>
		/// Create the view
		/// </summary>
		public abstract void BuildView();

		/// <summary>
		/// Updates the view of the cell
		/// </summary>
		public abstract void UpdateView ();

		#endregion

		#region Update cell events

		/// <summary>
		/// Cell has been selected (touched)
		/// </summary>
		public abstract void SelectCell ();

		/// <summary>
		/// Touch released
		/// </summary>
		public abstract void UnselectCell (bool success);

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

		/// <summary>
		/// Cell path
		/// </summary>
		/// <value>The path.</value>
		public Path Path {
			get;
			set;
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

