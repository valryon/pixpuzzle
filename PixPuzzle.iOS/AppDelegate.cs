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
			GameCenterHelper.Authenticate ((ui) => {
				InvokeOnMainThread (() => {
					Window.RootViewController.PresentViewController (ui, true, null);
				});
			}
			);

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
	}
}

