using System;
#if IOS
using System.Drawing;
#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;
#endif

namespace PixPuzzle.Data
{
	public abstract class Grid
	{
		/// <summary>
		/// Occurs when grid is completed.
		/// </summary>
		public event Action GridCompleted;

		public Grid ()
		{
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

