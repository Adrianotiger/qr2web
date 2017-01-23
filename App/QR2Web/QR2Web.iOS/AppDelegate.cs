using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ObjCRuntime;
using UIKit;
using CoreLocation;

namespace QR2Web.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication (new QR2Web.App ());

			InitExternalLibraries();
			InitOSSettings();
			ShowFullScreen();
			//LockPortrait(true); <- can't be set in this way on iOS

			return base.FinishedLaunching (app, options);
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			if (Scanner.IsAppURL(url.ToString()))
			{
				QR2Web.App.StartScanFromWeb(url.ToString());
			}
			return true;
		}

		public void ShowFullScreen()
		{
		}
		
		public void InitExternalLibraries()
		{
			ZXing.Net.Mobile.Forms.iOS.Platform.Init();
		}

		public void InitOSSettings()
		{
			var manager = new CLLocationManager();
			manager.RequestWhenInUseAuthorization();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, [Transient] UIWindow forWindow)
		{
			if(Parameters.Options.LockPortrait)
				return UIInterfaceOrientationMask.Portrait;
			else
				return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.Landscape;
		}
	}
}
