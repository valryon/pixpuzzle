using System;

namespace PixPuzzle.Data
{
	/// <summary>
	/// Simple grid cell
	/// </summary>
	public abstract class Cell
	{
		public Cell ()
		{
		}

		public Cell (int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Location (X)
		/// </summary>
		/// <value>The x.</value>
		public int X {
			get;
			set;
		}
		/// <summary>
		/// Location (Y)
		/// </summary>
		/// <value>The y.</value>
		public int Y {
			get;
			set;
		}
		/// <summary>
		/// Set the right color form the image
		/// </summary>
		/// <value>The color.</value>
		public CellColor Color {
			get;
			set;
		}
		/// <summary>
		/// Cells has been marked by grid creator
		/// </summary>
		/// <value><c>true</c> if this instance is marked; otherwise, <c>false</c>.</value>
		public bool IsMarked {
			get;
			set;
		}

		public override string ToString ()
		{
			return string.Format ("X:{0} Y:{1} Color:{2}", X, Y, Color);
		}
	}
}

