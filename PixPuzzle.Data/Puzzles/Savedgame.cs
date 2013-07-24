using System;
using System.Collections.Generic;

namespace PixPuzzle.Data
{

	[Serializable]
	public class Savedgame
	{
		public List<PuzzleData> Puzzles { get; set; }

		public Savedgame ()
		{
			Puzzles = new List<PuzzleData> ();
		}
	}
}

