using System;

namespace PixPuzzle.Data
{
    /// <summary>
    /// One cell of the picross puzzle
    /// </summary>
    public class PicrossCell : Cell
    {
		public PicrossCell ()
			: base()
		{
		}

        public PicrossCell(int x, int y)
			: base(x,y)
        {
        }

        #region Properties

        /// <summary>
        /// Tells if we are on a cell that must be filled
        /// </summary>
        /// <value><c>true</c> if this instance is path start or end; otherwise, <c>false</c>.</value>
        public bool ShoudBeFilled
        {
            get;
            set;
        }

		/// <summary>
		/// Tells if the cell is currently filled
		/// </summary>
		/// <value><c>true</c> if this instance is filled; otherwise, <c>false</c>.</value>
		public bool IsFilled
		{
			get;
			set;
		}

        #endregion
    }
}

