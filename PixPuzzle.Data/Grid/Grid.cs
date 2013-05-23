using System;

#if IOS
using System.Drawing;

#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;

#endif
namespace PixPuzzle.Data
{
	/// <summary>
	/// The base of the puzzle game: the grid, containing cell.
	/// The generic typing provides a way to store custom cell and the way it should be displayed.
	/// </summary>
	public abstract class Grid<TCell, TView> : IGrid
		where TCell : Cell, new()
		where TView : IGridView<TCell>
	{
		/// <summary>
		/// Occurs when grid is completed.
		/// </summary>
		public event Action GridCompleted;
		/// <summary>
		/// The grid
		/// </summary>
		protected TCell[][] Cells;

		public Grid (int imageWidth, int imageHeight, int cellSize)
		{
			CellSize = cellSize;
			Width = imageWidth;
			Height = imageHeight;
		}
		#region Grid creation

		/// <summary>
		/// Create a grid and initialize with default values
		/// </summary>
		/// <param name="createCell">Create cell.</param>
		public void CreateGrid (int locationX, int locationY, TView view)
		{
			// Create the grid
			Cells = new TCell[Width][];

			for (int x=0; x<Width; x++) {

				Cells [x] = new TCell[Height];

				for (int y=0; y<Height; y++) {

					TCell c = new TCell ();
					c.X = x;
					c.Y = y;
					Cells [x] [y] = c;
				}
			}

			// Initialize the view
			if (view == null)
				throw new ArgumentException ();

			this.View = view;

			BorderWidth = 4;
			GridLocation = new Point (0, 0);

			int borderStartX = GridLocation.X + (BorderWidth / 2);
			int borderStartY = GridLocation.Y + (BorderWidth / 2);
			BorderStartLocation = new Point (borderStartX, borderStartY);

			this.View.InitializeViewForDrawing ();
		}

		public abstract void SetupGrid ();
		#endregion

		#region View

		/// <summary>
		/// Request the grid to be updated, especially some cells that may have been modified
		/// </summary>
		/// <param name="cellsToUpdate">Cells to update.</param>
		public virtual void UpdateView (PathCell[] cellsToRefresh)
		{
			Rectangle zoneToRefresh = Rectangle.Empty;

			foreach (PathCell cell in cellsToRefresh) {

				int x = BorderStartLocation.X + cell.X * CellSize;
				int y = BorderStartLocation.Y + cell.Y * CellSize;

				Rectangle cellRect = new Rectangle (x, y, CellSize, CellSize);

				if (zoneToRefresh == Rectangle.Empty) {
					zoneToRefresh = cellRect;
				} else {
					#if IOS
					if (zoneToRefresh.Contains (cellRect) == false || zoneToRefresh.IntersectsWith (cellRect) == false) {
						#elif WINDOWS_PHONE
						if (zoneToRefresh.Contains (cellRect) == false || zoneToRefresh.Intersects (cellRect) == false) {
						#endif
						zoneToRefresh = Rectangle.Union (cellRect, zoneToRefresh);
					}
				}
			}

			// Trigger the refresh if necessary
			View.OrderRefresh (zoneToRefresh);
		}

		public abstract void DrawPuzzle ();
		#endregion

		#region Grid tools

		/// <summary>
		/// Get the cell from grid indices
		/// </summary>
		/// <returns>The cell.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public TCell GetCell (int x, int y)
		{
			if ((x >= 0 && x < Width) && (y >= 0 && y < Height)) {
				// Get the cell
				return Cells [x] [y];
			}

			return null;
		}
		#endregion

		protected void OnGridCompleted ()
		{
			if (GridCompleted != null) {
				GridCompleted ();
			}
		}
		/// <summary>
		/// View instance through a cross platform interface
		/// </summary>
		/// <value>The view.</value>
		public TView View {
			get;
			protected set;
		}

		public int CellSize { 
			get;
			private set; 
		}

		public int Width { 
			get;
			private set; 
		}

		public int Height { 
			get;
			private set; 
		}

		public int BorderWidth { 
			get;
			protected set; 
		}

		public Point GridLocation { 
			get;
			protected set; 
		}

		public Point BorderStartLocation { 
			get;
			protected set; 
		}
	}
}

