using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using PixPuzzle.Data;

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
			
//			ShowMenu ();
			ShowPuzzle (GameModes.Picross, "puzzles/0.png");
			return true;
		}

		public void ShowMenu() 
		{
			if (window.RootViewController != null) {
				window.RootViewController.View.RemoveFromSuperview ();
				window.RootViewController.Dispose ();
			}

			window.RootViewController = new MenuViewController();
			window.MakeKeyAndVisible ();
		}

		public void ShowPuzzle(GameModes mode, string selectedPuzzleFilename) 
		{
			if (window.RootViewController != null) {
				window.RootViewController.View.RemoveFromSuperview ();
				window.RootViewController.Dispose ();
			}

			window.RootViewController = new GameViewController(mode, selectedPuzzleFilename);
			window.MakeKeyAndVisible ();
		}
	}
}

