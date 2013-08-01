using System;

namespace PixPuzzle.Data
{
    /// <summary>
    /// One cell of the path puzzle
    /// </summary>
    public class PathCell
    {
		#region Constructor

		public PathCell ()
		{
		}

        public PathCell(int x, int y)
        {
			X = x;
			Y = y;

            // Default values
            Path = null;
            IsPathStartOrEnd = false;
        }

		#endregion

		#region Methods

        /// <summary>
        /// Sets the number to display. It also means that the cell is a path start or end.
        /// </summary>
        /// <param name="val">Value.</param>
        public void DefineCellAsPathStartOrEnd(int pathLength)
        {
            IsPathStartOrEnd = true;

            // The cell is the beginning or the end of a path
            Path = new Path(this, pathLength);
        }
		
		public override string ToString ()
		{
			return string.Format ("X:{0} Y:{1} Color:{2}", X, Y, Color);
		}

		#endregion

        #region Properties

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

        /// <summary>
        /// Tells if we are on a cell that is the start or the end of a complete path
        /// </summary>
        /// <value><c>true</c> if this instance is path start or end; otherwise, <c>false</c>.</value>
        public bool IsPathStartOrEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// Cell path
        /// </summary>
        /// <value>The path.</value>
        public Path Path
        {
            get;
            set;
        }

        #endregion
    }
}

