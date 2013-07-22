using System;
using System.Collections.Generic;

namespace PixPuzzle.Data
{
	[Serializable]
	public class PuzzleData
	{
		public string Filename;
		public int width;
		public int height;
		public DateTimeOffset? BestScore;
		public bool IsNew;
		public bool IsCustom;
		public string OwnerId;
	}

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

