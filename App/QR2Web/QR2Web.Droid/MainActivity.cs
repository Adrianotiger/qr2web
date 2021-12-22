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
using Android.Content;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Essentials;

namespace QR2Web.Droid
{
    [Activity(Label = "QR2Web",
        Icon = "@drawable/icon",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask)]
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "qr2web",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "readbarcode",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "p2spro",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    [IntentFilter(new[] { Intent.ActionView },
        DataScheme = "mochabarcode",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, OSInterface
    {
        bool intentDataRead = false;
        int oldViewHeight = -1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ShowFullScreen();

            Xamarin.Forms.Forms.Init(this, bundle);
            CrossCurrentActivity.Current.Init(this, bundle);

            LoadApplication(new App(this, false));

            InitOSSettings(bundle);
            InitExternalLibraries();
            
            LockPortrait(Parameters.Options.LockPortrait);

            intentDataRead = false;
            App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            View v = Window.DecorView;
            v.ViewTreeObserver.GlobalLayout += ViewTreeObserver_GlobalLayout;
        }

        /*Hack to resize browser size when kayboard is overlapping the form inside*/
        private void ViewTreeObserver_GlobalLayout(object sender, EventArgs e)
        {
            var newViewHeight = Window.FindViewById(Window.IdAndroidContent).Height;
            if(oldViewHeight != newViewHeight)
            {
                if(oldViewHeight < 0)
                {
                    oldViewHeight = newViewHeight;
                }
                else if(newViewHeight < oldViewHeight - 100)
                {
                    App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
                    oldViewHeight = newViewHeight;
                }
                else if (newViewHeight > oldViewHeight + 100)
                {
                    App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
                    oldViewHeight = newViewHeight;
                    new Task(() =>
                    {
                        Task.Delay(200);
                        App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
                    }).Start();
                }
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if(intent.Data != null)
            {
                if (!intentDataRead)
                {
                    intentDataRead = true;
                    Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        App.StartScanFromWeb(intent.Data.ToString());
                    });
                }
            }
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
            Xamarin.Essentials.Platform.Init(Application);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Rg.Plugins.Popup.Popup.Init(this);
        }

        public void InitOSSettings(Bundle bundle)
        {

        }

        public override void OnBackPressed()
        {
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
                Android.Net.Uri uri = Android.Net.Uri.Parse(url);
                Intent intent = new Intent(Intent.ActionView)
                        .SetData(uri)
                        .SetFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                success = false;
            }
            return success;
        }
    }
}

