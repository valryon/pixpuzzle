using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;
using MonoTouch.GameKit;

namespace PixPuzzle
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{	
		public static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		// class-level declarations
		public override UIWindow Window {
			get;
			set;
		}

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Logger.I ("Finished launching");

			// Game center
			GCAuthenticate ();

			// Savedgames
			PuzzleService.Instance.Initialize (
				"puzzles/path/",
				"pix.save"
			);

			return true;
		}

		public override void ReceiveMemoryWarning (UIApplication application)
		{
			Logger.E ("MEMORY WARNING");
		}


		#region Game Center
		/// <summary>
		/// Authenticate the player for Game Center
		/// </summary>
		public void GCAuthenticate ()
		{
			Logger.I ("Game Center Authentication requested...");

			GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) => {

				// If ui is null, that means the user is already authenticated,
				// for example, if the user used Game Center directly to log in
				if (ui != null) {
					InvokeOnMainThread (() => {
						Window.RootViewController.PresentViewController (ui, true, null);
					});
				} 

				if (error != null) {
					Logger.E ("Game Center Authentication failed! " + error);
				} else {
					if (GKLocalPlayer.LocalPlayer.Authenticated) {
						Logger.I ("Game Center - " + GKLocalPlayer.LocalPlayer.PlayerID + " (" + GKLocalPlayer.LocalPlayer.DisplayName + ")");
					} else {
						Logger.W ("Game Center - disabled !");
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
		public void NewVersusPhoto (Action<PuzzleData> matchFoundCallback, Action cancelCallback, Action errorCallback, Action playerQuitCallback)
		{
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

			InvokeOnMainThread (() => {
				Window.RootViewController.PresentViewController (matchMakerVc, true, null);
			});
		}
		#endregion
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

