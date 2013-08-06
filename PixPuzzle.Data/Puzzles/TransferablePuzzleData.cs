using System;

namespace PixPuzzle.Data
{
	/// <summary>
	/// Puzzle shared over network
	/// </summary>
	[Serializable]
	public class TransferablePuzzleData
	{
		public string Base64Image;
		public PuzzleData Puzzle;

		public TransferablePuzzleData ()
			: base()
		{
		}
	}
}

