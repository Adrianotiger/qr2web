using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;

namespace QR2Web.Droid
{
	[Activity (Label = "QR to Web Inventory", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init(this, bundle);
			
			LoadApplication (new QR2Web.App());

			InitOSSettings(bundle);
			InitExternalLibraries();
			ShowFullScreen();
			LockPortrait(Parameters.Options.LockPortrait);
		}

		public void ShowFullScreen()
		{
			this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Window.ClearFlags(WindowManagerFlags.Fullscreen);
		}

		public void LockPortrait(bool tryToLock)
		{
			if (tryToLock)
			{
				this.RequestedOrientation = ScreenOrientation.SensorPortrait;
			}
		}

		public void InitExternalLibraries()
		{
			//global::Xamarin.Forms.Forms.Init(this, bundle);
			ZXing.Net.Mobile.Forms.Android.Platform.Init();

			// Initialize the scanner first so we can track the current context 
			MobileBarcodeScanner.Initialize(Application);
		}

		public void InitOSSettings(Bundle bundle)
		{
			
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

