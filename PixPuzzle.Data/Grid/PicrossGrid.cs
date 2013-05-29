using System;
using System.Collections.Generic;

#if IOS
using System.Drawing;

#elif WINDOWS_PHONE
using Microsoft.Xna.Framework;


#endif
namespace PixPuzzle.Data
{
	public class PicrossSerieNumber
	{
		public int Value;
		public bool IsValid;
	}
	/// <summary>
	/// A line or column of the puzzle
	/// </summary>
	public class PicrossSerie
	{
		public List<PicrossSerieNumber> Numbers { get; private set; }

		public bool IsZero {
			get;
			private set;
		}

		public bool IsValid {
			get {

				foreach (var number in Numbers) {
					if (number.IsValid == false)
						return false;
				}

				return true;
			}
		}

		public PicrossSerie ()
		{
			Numbers = new List<PicrossSerieNumber> ();
		}

		public void SetZero ()
		{
			IsZero = true;
			Numbers.Add (new PicrossSerieNumber() {
				Value = 0,
				IsValid = true
			});
		}
		/// <summary>
		/// Compares the expected number pattern to the given one. 
		/// If it's the same then the serie will be valid.
		/// </summary>
		/// <param name="currentNumbers">Current numbers.</param>
		public void CompareToLineData (List<int> currentNumbers)
		{

			foreach (var number in Numbers) {

				// The 0 case
				if (number.Value == 0 && currentNumbers.Count == 0) {
					number.IsValid = true;
					return;
				}

				number.IsValid = false;
			}

			for (int i=0; i< Numbers.Count; i++) {

				// A part of the serie can be valid
				if (currentNumbers.Count > i) {
					if (Numbers [i].Value == currentNumbers [i]) {
						Numbers [i].IsValid = true;
					}
				}
			}
		}

		public override string ToString ()
		{
			string s = " ";

			foreach (var number in Numbers) {
				s += number.Value + " ";
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

		private List<int> revealedLines, revealedCols;

		public PicrossGrid (int imageWidth, int imageHeight, int cellSize)
			: base(imageWidth, imageHeight, cellSize)
		{
			revealedLines = new List<int> ();
			revealedCols = new List<int> ();
		}

		public void SetFilledPixel (int x, int y)
		{
			PicrossCell cell = GetCell (x, y);

			if (cell != null) {
				cell.ShouldBeFilled = true;
			}
		}

		public void RevealLine (int y)
		{
			revealedLines.Add (y);

			for (int x=0; x<Width; x++) {
				PicrossCell cell = GetCell (x, y);

				if (cell != null) {
					if (cell.ShouldBeFilled) {
						cell.State = PicrossCellState.Filled;
					} else {
						cell.State = PicrossCellState.Crossed;
					}
				}
			}
		}

		public void RevealCol (int x)
		{
			revealedCols.Add (x);

			for (int y=0; y<Height; y++) {
				PicrossCell cell = GetCell (x, y);

				if (cell != null) {
					if (cell.ShouldBeFilled) {
						cell.State = PicrossCellState.Filled;
					} else {
						cell.State = PicrossCellState.Crossed;
					}
				}
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
			for (int y=0; y< Height; y++) {

				Lines [y] = new PicrossSerie ();

				FirstCountLine (y);
			}

			// - Columns
			Columns = new PicrossSerie[Width];
			for (int x=0; x<Width; x++) {

				Columns [x] = new PicrossSerie ();

				FirstCountCol (x);
			}

			// Reveal some lines and columns
			int linesToReveal = (Width / 10);
			int colsToReveal = (Height / 10);
			Random random = new Random (DateTime.Now.Millisecond);

			Console.WriteLine (string.Format("Revealing {0} lines and {1} columns.", linesToReveal, colsToReveal));

			while (linesToReveal > 0 && revealedLines.Count < Width) {

				int randomLine = random.Next (Lines.Length);

				if(revealedLines.Contains(randomLine) == false) {
					RevealLine (randomLine);
					linesToReveal--;
				}
			}

			while (colsToReveal > 0 && revealedCols.Count < Height) {

				int randomCol = random.Next (Columns.Length);

				if(revealedLines.Contains(randomCol) == false) {
					RevealCol (randomCol);
					colsToReveal--;
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

		public void FirstCountLine (int y)
		{
			// Look to each cell of the line and deduce the numbers (1, 3, 2 for example)
			int expectedCount = 0;
			for (int x=0; x<Width; x++) {

				PicrossCell cell = Cells [x] [y];
				cell.IsMarked = true;

				if (cell.ShouldBeFilled) {
					expectedCount++;
				} else {
					if (expectedCount > 0) {

						Lines [y].Numbers.Add (new PicrossSerieNumber() {
							Value = expectedCount
						});

						expectedCount = 0;
					}
				}
			}

			if (Lines [y].Numbers.Count == 0) {
				Lines [y].SetZero ();
				RevealLine (y);
			}
		}

		public void FirstCountCol (int x)
		{
			int expectedCount = 0;
			for (int y=0; y<Height; y++) {

				PicrossCell cell = Cells [x] [y];
				cell.IsMarked = true;

				if (cell.ShouldBeFilled) {
					expectedCount++;
				} else {
					if (expectedCount > 0) {

						Columns [x].Numbers.Add (new PicrossSerieNumber() {
							Value = expectedCount
						});

						expectedCount = 0;
					}
				}
			}

			if (Columns [x].Numbers.Count == 0) {
				Columns [x].SetZero ();
				RevealCol (x);
			}
		}

		public void CheckLine (int y)
		{
			int currentCount = 0;
			int currentNumber = 0;

			List<int> lineData = new List<int> ();

			for (int x=0; x<Width; x++) {

				PicrossCell cell = Cells [x] [y];

				if (cell.State == PicrossCellState.Filled) {
					currentCount++;
				} else {
					if (currentCount > 0) {

						lineData.Add (currentCount);

						currentNumber++;
						currentCount = 0;
					}
				}
			}

			Lines [y].CompareToLineData (lineData);
		}

		public void CheckColumn (int x)
		{
			int currentCount = 0;
			int currentNumber = 0;

			List<int> lineData = new List<int> ();

			for (int y=0; y<Height; y++) {

				PicrossCell cell = Cells [x] [y];

				if (cell.State == PicrossCellState.Filled) {
					currentCount++;
				} else {
					if (currentCount > 0) {

						lineData.Add (currentCount);

						currentNumber++;
						currentCount = 0;
					}
				}
			}

			Columns [x].CompareToLineData (lineData);
		}
		/// <summary>
		/// Changes the state of the cell.
		/// </summary>
		/// <returns><c>true</c>, if cell state was changed, <c>false</c> otherwise.</returns>
		/// <param name="cell">Cell.</param>
		/// <param name="state">State.</param>
		public void ChangeCellState (PicrossCell cell, PicrossCellState state)
		{
			if (cell != null && cell != LastSelectedCell) {

				Console.WriteLine ("Cell X:"+cell.X+ " Y:"+cell.Y);
				Console.WriteLine ("Changing state: "+state+ " filled:"+cell.ShouldBeFilled);

				// Change cell state
				cell.State = state;

				// Keep track of the cell we just modified
				LastSelectedCell = cell;

				// Update numbers
				// Get the line and the column
				CheckLine (cell.Y);
				CheckColumn (cell.X);

				// Tell the view to refresh
				UpdateView (new PicrossCell[]{ cell });

				// Check if grid is completed
				bool valid = true;
				foreach (PicrossSerie line in Lines) {
					valid &= line.IsValid;

					if (valid == false)
						break;
				}

				if (valid) {
					foreach (PicrossSerie col in Columns) {
						valid &= col.IsValid;

						if (valid == false)
							break;
					}
				}

				if (valid) {
					OnGridCompleted ();
				}
			}
		}

		public void InputEnded ()
		{
			LastSelectedCell = null;
		}
		#endregion
	}
}

