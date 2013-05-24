using System;
using System.Collections.Generic;

#if IOS
using System.Drawing;

#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;




#endif
namespace PixPuzzle.Data
{
	/// <summary>
	/// A number from a line/column of the puzzle
	/// </summary>
	public struct PicrossSerieNumber
	{
		public int Count;
		public int CurrentCount;
	}
	/// <summary>
	/// A line or column of the puzzle
	/// </summary>
	public class PicrossSerie
	{
		public List<PicrossSerieNumber> Numbers { get; private set; }

		public bool IsValid {
			get {
				bool isValid = true;
				foreach (PicrossSerieNumber number in Numbers) {
					isValid &= (number.Count == number.CurrentCount);

					if (isValid == false)
						return false;
				}

				return isValid;
			}
		}

		public PicrossSerie ()
		{
			Numbers = new List<PicrossSerieNumber> ();
		}

		public override string ToString ()
		{
			string s = " ";

			foreach (PicrossSerieNumber number in Numbers) {
				s += number.Count + " ";
			}

			return s;
		}
	}
	/// <summary>
	/// Picross-like grid.
	/// </summary>
	public class PicrossGrid : Grid<PicrossCell, IPicrossGridView>
	{
		protected PicrossCell LastSelectedCell;

		public PicrossSerie[] Lines { get; private set; }

		public PicrossSerie[] Columns { get; private set; }

		public PicrossGrid (int imageWidth, int imageHeight, int cellSize)
			: base(imageWidth, imageHeight, cellSize)
		{
		}

		public void SetFilledPixel (int x, int y)
		{
			PicrossCell cell = GetCell (x, y);

			if (cell != null) {
				cell.ShoudBeFilled = true;
			}
		}

		public override void SetupGrid (CellColor[][] pixels)
		{
			for (int x=0; x<pixels.Length; x++) {
				for (int y=0; y<pixels[x].Length; y++) {

					// Get the pixel color
					CellColor c = pixels [x] [y];

					bool isFilled = false;

					float rgb = 0f;
					rgb += c.R + c.G + c.B;

					if (c.A > 0.2f) {
						isFilled = (rgb < 0.20f);
					}

					if (isFilled) {
						SetFilledPixel (x, y);
					}
				}	
			}

			// So we can deduce the grid and get the lines and columns nmbers
			// -- Lines
			Lines = new PicrossSerie[Height];
			int currentCount = 0;
			for (int y=0; y< Height; y++) {

				Lines [y] = new PicrossSerie ();

				// Look to each cell of the line and deduce the numbers (1, 3, 2 for example)
				currentCount = 0;
				for (int x=0; x<Width; x++) {

					PicrossCell cell = Cells [x] [y];

					if (cell.ShoudBeFilled) {
						currentCount++;
					} else {
						if (currentCount > 0) {

							Lines [y].Numbers.Add (new PicrossSerieNumber() {
								Count = currentCount
							});

							currentCount = 0;
						}
					}
				}

			}

			// - Columns
			Columns = new PicrossSerie[Width];
			currentCount = 0;
			for (int x=0; x<Width; x++) {

				Columns [x] = new PicrossSerie ();

				// Same logic than for lines, obviously
				currentCount = 0;
				for (int y=0; y<Height; y++) {

					PicrossCell cell = Cells [x] [y];

					if (cell.ShoudBeFilled) {
						currentCount++;
					} else {
						if (currentCount > 0) {

							Columns [x].Numbers.Add (new PicrossSerieNumber() {
								Count = currentCount
							});

							currentCount = 0;
						}
					}
				}
			}

			base.SetupGrid (pixels);
		}

		public override void DrawPuzzle ()
		{
			View.StartDraw ();

			// Draw grid and background
			// ------------------------------------------------------------
			View.DrawGrid ();

			// Draw each cell 
			// ------------------------------------------------------------
			for (int x=0; x<Width; x++) {
				for (int y=0; y<Height; y++) {

					PicrossCell cell = GetCell (x, y);

					// Doesn't exists?
					if (cell == null)
						continue;

					// Get borders
					int cellStartX = GridLocation.X + (x * CellSize);
					int cellStartY = GridLocation.Y + (y * CellSize);
					Rectangle cellRect = new Rectangle (cellStartX, cellStartY, CellSize, CellSize);

					// Check if the cell has to be refreshed
					if (View.IsToRefresh (cell, cellRect) == false)
						continue;

					View.DrawCellBase (cell, cellRect);
				}
			}

			View.EndDraw ();
		}
		#region Game

		public bool FillCell (PicrossCell cell, bool isFilled, bool isCrossed)
		{
			if (cell != null && cell != LastSelectedCell) {

				Console.WriteLine ("Cell X:"+cell.X+ " Y:"+cell.Y);
				Console.WriteLine ("Changing state filled: "+isFilled+ " crossed:"+isCrossed);

				cell.IsFilled = isFilled;
				cell.IsCrossed = isCrossed;

				LastSelectedCell = cell;

				UpdateView (new PicrossCell[]{ cell });

				Console.WriteLine ("Cell isValid:"+(cell.ShoudBeFilled == cell.IsFilled));

				return cell.ShoudBeFilled == cell.IsFilled;
			}

			return false;
		}
		#endregion
	}
}

