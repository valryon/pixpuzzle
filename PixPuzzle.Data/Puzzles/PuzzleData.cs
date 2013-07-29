using System;

namespace PixPuzzle.Data
{
	[Serializable]
	public class PuzzleData
	{
		public string Filename;
		public DateTime? BestScore;
		public bool IsNew;
		public bool IsCustom;
		public string OwnerId;

#if IOS
		/// <summary>
		/// For versus
		/// </summary>
		/// <value>The match.</value>
		[NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		private MonoTouch.GameKit.GKTurnBasedMatch _match;

		[System.Xml.Serialization.XmlIgnore]
		public MonoTouch.GameKit.GKTurnBasedMatch Match
		{
			get { return _match;}
			set { _match = value;}
		}
#endif
	}
}

