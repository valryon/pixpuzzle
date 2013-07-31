using System;
using MonoTouch.GameKit;
using PixPuzzle.Data;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace PixPuzzle
{
	public static class GameCenterHelper
	{
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

						PuzzleData newPuzzle = new PuzzleData ();

						// TODO Deserialize match data

						newPuzzle.MatchId = match.MatchID;
						newPuzzle.Match = match;

						puzzles.Add (newPuzzle);
					}

					if (listLoaded != null) {
						listLoaded (puzzles);
					}
				}
			});
		}

		public static bool IsAuthenticated
		{
			get {
				return GKLocalPlayer.LocalPlayer.Authenticated;
			}
		}

		/// <summary>
		/// Gets the local game kit player.
		/// </summary>
		/// <returns>The local player.</returns>
		public static GKPlayer LocalPlayer
		{
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

				// Match has data
				if (match.MatchData.Length > 0) {
					//					VersusMatch existingMatch = new VersusMatch ();
					//	
					//					try {
					//	
					//						string jsonBase64 = NSString.FromData (match.MatchData, NSStringEncoding.UTF8);
					//						string json = System.Text.Encoding.UTF8.GetString (Convert.FromBase64String (jsonBase64));
					//	
					//						existingMatch.FromJson (json.ToString ());
					//						this.parent.CurrentMatch = existingMatch;
					//					} catch (Exception e) {
					//						matchError = true;
					//						Logger.LogException (LogLevel.Error, "GameCenterPlayer.FoundMatch", e);
					//					}
				}
				// No data: new match, 
				else {
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

