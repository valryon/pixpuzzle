using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using PixPuzzle.Data;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;
using MonoTouch.GameKit;

namespace PixPuzzle
{
	public partial class MenuViewController : UIViewController
	{
		private UIView buttonPanel, levelSelectionPanel;

		public MenuViewController (IntPtr handle) : base (handle)
		{

		}


		#region Events

		partial void OnPlayButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var vc = this.Storyboard.InstantiateViewController("MenuPlayViewController") as UIViewController;

			NavigationController.PushViewController(
				vc,
				true
				);
		}

		partial void OnCreateButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var vc = this.Storyboard.InstantiateViewController("MenuCreateViewController") as UIViewController;

			NavigationController.PushViewController(
				vc,
				true
				);
		}

		partial void OnCreditsButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var vc = this.Storyboard.InstantiateViewController("MenuCreditsViewController") as UIViewController;

			NavigationController.PushViewController(
				vc,
				true
				);
		}

		partial void OnFriendsButtonPressed (MonoTouch.Foundation.NSObject sender)
		{
			var vc = this.Storyboard.InstantiateViewController("MenuFriendsViewController") as UIViewController;

			NavigationController.PushViewController(
				vc,
				true
				);
		}

		#endregion
	}
}

