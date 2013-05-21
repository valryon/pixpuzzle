using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public abstract class Grid<TCell> 
		where TCell : Cell, new()
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
		public void CreateGrid (int locationX, int locationY, IGridView view)
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
		#endregion

		protected void OnGridCompleted() {
			if (GridCompleted != null) {
				GridCompleted ();
			}
		}

		public IGridView View {
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

