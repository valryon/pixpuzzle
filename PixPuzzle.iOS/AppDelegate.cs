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
		private UIWindow window;
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
		
			Logger.I ("Finished launching");

			// Game center
			GCAuthenticate ();

			// Savedgames
			PuzzleService.Instance.Initialize (
				"puzzles/path/",
				"pix.save"
			);

			// UI
			ShowMenu ();

			return true;
		}

		public void ShowMenu ()
		{
			if (window.RootViewController != null) {
				window.RootViewController.View.RemoveFromSuperview ();
				window.RootViewController.Dispose ();
			}

			window.RootViewController = new MenuViewController ();
			window.MakeKeyAndVisible ();
		}

		public void ShowPuzzle (PuzzleData puzzle, UIImage selectedPuzzle)
		{
			if (window.RootViewController != null) {
				window.RootViewController.View.RemoveFromSuperview ();
				window.RootViewController.Dispose ();
			}

			window.RootViewController = new GameViewController (puzzle, selectedPuzzle);
			window.MakeKeyAndVisible ();
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
						window.RootViewController.PresentViewController (ui, true, null);
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
		#endregion
	}
}

