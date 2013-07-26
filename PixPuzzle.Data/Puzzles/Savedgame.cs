using System;
using System.Collections.Generic;

namespace PixPuzzle.Data
{
	[Serializable]
	public class Savedgame
	{
		public Savedgame ()
		{
			Puzzles = new List<PuzzleData> ();
		}

		public List<PuzzleData> Puzzles { get; set; }
	}
}

