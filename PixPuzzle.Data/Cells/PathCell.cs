using System;

namespace PixPuzzle.Data
{
    /// <summary>
    /// One cell of the path puzzle
    /// </summary>
    public class PathCell : Cell
    {
		public PathCell ()
			: base()
		{
		}

        public PathCell(int x, int y)
			: base(x,y)
        {
            // Default values
            Path = null;
            IsPathStartOrEnd = false;
        }

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

        #region Properties

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

