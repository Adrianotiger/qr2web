using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Essentials;

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
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, OSInterface
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

			LoadApplication (new QR2Web.App (this, false));

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

		public async void EnablePermissionLocation()
		{
			await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
		}

		public bool OpenExternalUrl(string url)
		{
			bool success = true;
			try
			{
				/*
				Android.Net.Uri uri = Android.Net.Uri.Parse(url);
				Intent intent = new Intent(Intent.ActionView)
						.SetData(uri)
						.SetFlags(ActivityFlags.NewTask);
				StartActivity(intent);
				*/
				Launcher.OpenAsync(url);
			}
			catch (Exception ex)
			{
				//Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
				success = false;
			}
			return success;
		}
	}
}
