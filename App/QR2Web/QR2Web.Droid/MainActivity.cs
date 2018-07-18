using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;
using System.Threading.Tasks;
using Plugin.CurrentActivity;

namespace QR2Web.Droid
{
	[Activity(Label = "QR2Web Inventory",
		Icon = "@drawable/icon",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		LaunchMode = LaunchMode.SingleTask)]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
		DataScheme = "qr2web",
		Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
		DataScheme = "readbarcode",
		Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
		DataScheme = "p2spro",
		Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
		DataScheme = "mochabarcode",
		Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		bool intentDataRead = false;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Xamarin.Forms.Forms.Init(this, bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            LoadApplication(new QR2Web.App());

			InitOSSettings(bundle);
			InitExternalLibraries();
			ShowFullScreen();
			LockPortrait(Parameters.Options.LockPortrait);

			intentDataRead = false;
		}

		protected override void OnStart()
		{
			base.OnStart();
			
			if (Intent.Data != null)
			{
				if (!intentDataRead)
				{
					intentDataRead = true;
					Task.Run(async () =>
					{
						await Task.Delay(500);
						App.StartScanFromWeb(Intent.Data.ToString());
					});
				}
			}
		}

		public void ShowFullScreen()
		{
			// no full screen on desktop PC
			if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Phone)
			{
				this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			}
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
			//global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

