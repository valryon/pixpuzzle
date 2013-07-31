using System;
using System.Linq;
using System.Collections.Generic;

namespace PixPuzzle.Data
{
	[Serializable]
	public struct PuzzleScoreLine
	{
		public string PlayerId;
		public bool IsLocal;
		public DateTime Time;

		public PuzzleScoreLine (string playerId, bool isLocal, DateTime time)
		{
			PlayerId = playerId;
			IsLocal = isLocal;
			Time = time;
		}
	}

	[Serializable]
	public class PuzzleData
	{
		public string Filename;
		public List<PuzzleScoreLine> Scores;
		public bool IsNew;
		public bool IsCustom;
		public string OwnerId;
		public string MatchId;

		public PuzzleData ()
		{
			Scores = new List<PuzzleScoreLine> ();
		}

		/// <summary>
		/// Add a new score for a player
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="time">Time.</param>
		public void AddPlayerScore (string id, bool isLocal, DateTime time)
		{
			Scores.Add (new PuzzleScoreLine (id, isLocal, time));
		}

		/// <summary>
		/// Get the best score for a player
		/// </summary>
		/// <returns>The player score.</returns>
		/// <param name="id">Identifier.</param>
		public DateTime? GetBestPlayerScore (string id)
		{
			var score = Scores.Where (p => p.PlayerId == id).OrderBy (s => s.Time);

			if (score.Any ()) {
				return score.First ().Time;
			}

			return null;
		}

		#if IOS

		/// <summary>
		/// For versus
		/// </summary>
		/// <value>The match.</value>
		[NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		private MonoTouch.GameKit.GKTurnBasedMatch _match;

		[System.Xml.Serialization.XmlIgnore]
		public MonoTouch.GameKit.GKTurnBasedMatch Match {
			get { return _match;}
			set { _match = value;}
		}

		#endif
	}
}

