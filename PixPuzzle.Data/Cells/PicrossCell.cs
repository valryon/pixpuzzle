using System;

namespace PixPuzzle.Data
{
	public enum PicrossCellState
	{
		None,
		Filled,
		Crossed
	}

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
        /// Tells if we are on a cell that must be filled to complete the puzzle
        /// </summary>
        /// <value><c>true</c> if this instance is path start or end; otherwise, <c>false</c>.</value>
        public bool ShouldBeFilled
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
		public PicrossCellState State
		{
			get;
			set;
		}

		public bool IsValid 
		{
			get {
				if (ShouldBeFilled) {
					return State == PicrossCellState.Filled;
				} else {
					return State != PicrossCellState.Filled;
				}
			}
		}

        #endregion
    }
}

