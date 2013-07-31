using System;
using MonoTouch.GameKit;
using PixPuzzle.Data;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Xml.Serialization;
using System.IO;

namespace PixPuzzle
{
	public static class GameCenterHelper
	{

		public static void OnInvititationReceived (GKInvite invite, string[] players)
		{
			Logger.I ("Invitaion received: " + invite);
		}

		/// <summary>
		/// Get player friends id
		/// </summary>
		/// <returns>The friends.</returns>
		public static string[] GetFriends() 
		{
			if (GKLocalPlayer.LocalPlayer.Authenticated) {
				return GKLocalPlayer.LocalPlayer.Friends;
			}

			return null;
		}

		/// <summary>
		/// Authenticate the player for Game Center
		/// </summary>
		public static void GCAuthenticate (Action<UIViewController> showCallback)
		{
			Logger.I ("Game Center: authentication requested...");

			GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) => {

				// If ui is null, that means the user is already authenticated,
				// for example, if the user used Game Center directly to log in
				if (ui != null) {
					if (showCallback != null) {
						showCallback (ui);
					}
				} 

				if (error != null) {
					Logger.E ("Game Center: authentication failed! " + error);
				} else {
					if (GKLocalPlayer.LocalPlayer.Authenticated) {
						Logger.I ("Game Center: " + GKLocalPlayer.LocalPlayer.PlayerID + " (" + GKLocalPlayer.LocalPlayer.DisplayName + ")");
					} else {
						Logger.W ("Game Center: disabled !");
					}
				}
			};
		}

		/// <summary>
		/// Send a photo to a friend via GameCenter
		/// </summary>
		/// <param name="matchFoundCallback">Match found callback.</param>
		/// <param name="cancelCallback">Cancel callback.</param>
		/// <param name="errorCallback">Error callback.</param>
		/// <param name="playerQuitCallback">Player quit callback.</param>
		public static void NewVersusPhoto (Action<UIViewController> showCallback, Action<PuzzleData> matchFoundCallback, Action cancelCallback, Action errorCallback, Action playerQuitCallback)
		{
			Logger.I ("Game center: new turn based match request...");

			GKMatchRequest matchRequest = new GKMatchRequest ();
			matchRequest.MinPlayers = 2;
			matchRequest.MaxPlayers = 2;
			matchRequest.DefaultNumberOfPlayers = 2;

			GKTurnBasedMatchmakerViewController matchMakerVc = new GKTurnBasedMatchmakerViewController (matchRequest);
			var d = new MatchMakerDelegate ();
			d.MatchFoundCallback += matchFoundCallback;
			d.CancelCallback += cancelCallback;
			d.ErrorCallback += errorCallback;
			d.PlayerQuitCallback += playerQuitCallback;

			matchMakerVc.Delegate = d;

			if (showCallback != null) {
				showCallback (matchMakerVc);
			}

		}

		/// <summary>
		/// Update versus match status
		/// </summary>
		public static void UpdateMatchFromPuzzle (PuzzleData puzzle, Action<NSError> callback)
		{
			if (puzzle.Match == null) {
				Logger.E ("This puzzle contains no match data... Can't do anything!");
				return;
			}

			// Match data -> Puzzle + Image
			// -- Image as Base64 string
			UIImage image = UIImage.FromFile (puzzle.Filename);
			Byte[] byteArray = null; 
			using (NSData nsImageData = image.AsPNG()) { 
				byteArray = new Byte[nsImageData.Length]; 
				System.Runtime.InteropServices.Marshal.Copy (nsImageData.Bytes, byteArray, 0, Convert.ToInt32 (nsImageData.Length)); 
			} 

			string base64image = Convert.ToBase64String (byteArray);

			// -- Data
			TransferablePuzzleData tp = new TransferablePuzzleData ();
			tp.Puzzle = puzzle;
			tp.Base64Image = base64image;

			// Build an XML text
			string xml = string.Empty;
			XmlSerializer xmlSerializer = new XmlSerializer (tp.GetType ());

			using (StringWriter textWriter = new StringWriter()) {
				xmlSerializer.Serialize (textWriter, tp);
				xml = textWriter.ToString ();
			}

			xmlSerializer = null;

			// Xml to bytes
			NSData puzzleData = NSData.FromString (xml);

			// Next participant
			GKTurnBasedParticipant nextPlayer = null;
			foreach (var participant in puzzle.Match.Participants) {
				if (participant.PlayerID != GKLocalPlayer.LocalPlayer.PlayerID) {
					nextPlayer = participant;
					break;
				}
			}

			// Game center match progress
			// -- Already an opponent score? End
			if (puzzle.GetBestPlayerScore (nextPlayer.PlayerID).HasValue) {
				puzzle.Match.EndMatchInTurn (
					puzzleData,
					(err) => {
					if (callback != null) {
						callback (err);
					}
				}
				);
			} else {
				// -- Send current score
				puzzle.Match.EndTurn (
					new GKTurnBasedParticipant[] { nextPlayer },
					120,
					puzzleData,
					callback
				);
			}
		}

		/// <summary>
		/// List matches with friends
		/// </summary>
		/// <param name="listLoaded">List loaded.</param>
		public static void GetFriendsPuzzles (Action<List<PuzzleData>> listLoaded)
		{
			Logger.I ("Game center: requesting turn based matches...");

			GKTurnBasedMatch.LoadMatches ((matches, error) => {

				if (error != null) {
					Logger.E ("Game Center: match list failed... ", error);
				} else {

					Logger.I ("Game center: " + matches.Length + " turn based matches found.");

					List<PuzzleData> puzzles = new List<PuzzleData> ();

					foreach (GKTurnBasedMatch match in matches) {

						// Deserialize data
						TransferablePuzzleData tp = GetPuzzleFromMatch (match);

						if (tp != null) {
							tp.Puzzle.MatchId = match.MatchID;
							tp.Puzzle.Match = match;

							puzzles.Add (tp.Puzzle);
						}
					}

					if (listLoaded != null) {
						listLoaded (puzzles);
					}
				}
			});
		}

		/// <summary>
		/// Read and parse match data to extract match information.
		/// </summary>
		/// <returns>The puzzle from match.</returns>
		/// <param name="match">Match.</param>
		public static TransferablePuzzleData GetPuzzleFromMatch (GKTurnBasedMatch match)
		{
			// Filter obviously invalid matches
			if (match.MatchData == null || match.MatchData.Length == 0) {
				Logger.E ("Match without data! Maybe an old one? Shouldn't happend in production.");
				return null;
			}

			try {
				// Try to get the XML inside
				string xml = NSString.FromData (match.MatchData, NSStringEncoding.UTF8);

				TransferablePuzzleData tp = null;

				XmlSerializer xmlSerializer = new XmlSerializer (typeof(TransferablePuzzleData));

				using (StringReader reader = new StringReader(xml)) {
					tp = (TransferablePuzzleData)xmlSerializer.Deserialize (reader);
				}

				xmlSerializer = null;

				// Save the image if we don't have it locally
				if (tp != null) {
					if (string.IsNullOrEmpty (tp.Base64Image) == false) {
						if (File.Exists (tp.Puzzle.Filename) == false) {

							byte[] imgRaw = Convert.FromBase64String (tp.Base64Image);
							NSData imageData = NSData.FromArray (imgRaw);
							UIImage img = UIImage.LoadFromData (imageData);

							NSError err = null;
							img.AsPNG ().Save (tp.Puzzle.Filename, false, out err);
						}
					}
				}

				return tp;
			} catch (Exception e) {
				Logger.E ("GameCenterPlayer.FoundMatch", e);

				return null;
			}
		}

		public static bool IsAuthenticated {
			get {
				return GKLocalPlayer.LocalPlayer.Authenticated;
			}
		}

		/// <summary>
		/// Gets the local game kit player.
		/// </summary>
		/// <returns>The local player.</returns>
		public static GKPlayer LocalPlayer {
			get {
				return GKLocalPlayer.LocalPlayer;
			}
		}

		/// <summary>
		/// Game center match handler
		/// </summary>
		private class MatchMakerDelegate : GKTurnBasedMatchmakerViewControllerDelegate
		{
			public event Action<PuzzleData> MatchFoundCallback;
			public event Action CancelCallback, ErrorCallback, PlayerQuitCallback;

			public MatchMakerDelegate ()
			{
			}

			public override void WasCancelled (GKTurnBasedMatchmakerViewController viewController)
			{
				Logger.I ("MatchMakerDelegate.WasCancelled");

				viewController.DismissViewController (true, null);

				if (CancelCallback != null)
					CancelCallback ();
			}

			public override void FailedWithError (GKTurnBasedMatchmakerViewController viewController, MonoTouch.Foundation.NSError error)
			{
				Logger.W ("MatchMakerDelegate.FailedWithError");

				viewController.DismissViewController (true, null);

				if (ErrorCallback != null)
					ErrorCallback ();
			}

			public override void FoundMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match)
			{
				Logger.I ("MatchMakerDelegate.FoundMatch");

				viewController.DismissViewController (true, null);

				PuzzleData puzzleData = new PuzzleData ();
				puzzleData.Match = match;
				puzzleData.MatchId = match.MatchID;

				bool matchError = false;

				if (match.MatchData.Length > 0) {
					// Match has data
//					var tp = GameCenterHelper.GetPuzzleFromMatch (match);
					// TODO ?
				} else {
					// No data: new match

					// Set up outcomes
					// -> Player who sent the picture set a time first
					match.Participants [0].MatchOutcome = GKTurnBasedMatchOutcome.First;
					match.Participants [1].MatchOutcome = GKTurnBasedMatchOutcome.Second;
				}

				if (matchError == false) {
					match.Remove (new GKNotificationHandler ((e) => {}));

					if (MatchFoundCallback != null) {
						MatchFoundCallback (puzzleData);
					}
				} else {
					if (ErrorCallback != null) {
						ErrorCallback ();
					}
				}
			}

			public override void PlayerQuitForMatch (GKTurnBasedMatchmakerViewController viewController, GKTurnBasedMatch match)
			{
				Logger.I ("MatchMakerDelegate.PlayerQuitForMatch");

				// Mark current player as quiter
				foreach (GKTurnBasedParticipant participant in match.Participants) {
					if (participant.PlayerID == GKLocalPlayer.LocalPlayer.PlayerID) {
						participant.MatchOutcome = GKTurnBasedMatchOutcome.Quit;
					} else {
						// Win?
						participant.MatchOutcome = GKTurnBasedMatchOutcome.Won;
					}
				}

				//viewController.DismissViewController (true, null);

				// Delete the match
				match.Remove (new GKNotificationHandler ((error) => {
					Logger.E ("MatchMakerDelegate.PlayerQuitForMatch: " + error.DebugDescription);
				}));

				if (PlayerQuitCallback != null)
					PlayerQuitCallback ();
			}
		}
	}
}

