using System;

using Android.App;
using Android.Content.PM;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;
using System.Threading.Tasks;
using Android.Content;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Essentials;
using Android.Window;
using AndroidX.AppCompat.App;
using Xamarin.Forms.Platform.Android;
using Android.Bluetooth;
//using Xamarin.Forms;

namespace QR2Web.Droid
{
    [Activity(Label = "QR2Web",
        Icon = "@drawable/icon",
        Theme = "@style/MyTheme.QR",
        MainLauncher = false,
        Exported = true,
        NoHistory = false,
        AlwaysRetainTaskState = true,
        HardwareAccelerated = true,
        ResumeWhilePausing = true,
        ClearTaskOnLaunch = false,
        ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustPan,/*
        StateNotNeeded = false,
        UiOptions = UiOptions.SplitActionBarWhenNarrow,
        FinishOnTaskLaunch = false,
        DocumentLaunchMode = DocumentLaunchMode.IntoExisting,*/
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
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
    public class MainActivity : /*global::Xamarin.Forms.Platform.Android.FormsApplicationActivity*/ global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, OSInterface
    {
        bool intentDataRead = false;
        int oldViewHeight = -1;
        HybridWebViewRenderer _myWebView = null;
        Bundle _savedBundle = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Console.WriteLine("[QR2WEB Task] - CREATE " + bundle != null ? " WITH BUNDLE" : ".");

            ShowFullScreen();

            Xamarin.Forms.Forms.Init(this, bundle);

            _savedBundle = bundle;

            InitOSSettings(bundle);
            InitExternalLibraries();

            LoadApplication(new App(this, false));

            LockPortrait(Parameters.Options.LockPortrait);

            intentDataRead = false;
            App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            View v = Window.DecorView;
            v.ViewTreeObserver.GlobalLayout += ViewTreeObserver_GlobalLayout;
            Xamarin.Essentials.Platform.ActivityStateChanged += Platform_ActivityStateChanged;
            BackPressed += MainActivity_BackPressed;

            if (_myWebView != null && bundle != null)
            {
                _myWebView.RestoreView(bundle);
            }
        }

        public void InitExternalLibraries()
        {
            //Xamarin.Essentials.Platform.Init(Application);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Rg.Plugins.Popup.Popup.Init(this);
        }

        public void InitOSSettings(Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(this, bundle);
        }


        private void Platform_ActivityStateChanged(object sender, ActivityStateChangedEventArgs e)
        {
            Console.WriteLine("[QR2WEB Task] - STATE CHANGE - " + e.State.ToString());
        }

        public void SetWebView(HybridWebViewRenderer webview)
        {
            _myWebView = webview;
            if(_savedBundle != null) 
            { 
                _myWebView.RestoreView(_savedBundle);
            }
        }

        
        protected override void OnPause()
        {
            Console.WriteLine("[QR2WEB Task] - PAUSE");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("[QR2WEB Task] - RESUME");
            base.OnResume();
        }


        protected override void OnStop()
        {
            Console.WriteLine("[QR2WEB Task] - STOP");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("[QR2WEB Task] - DESTROY");
            base.OnDestroy();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("[QR2WEB Task] - RESTART");
            base.OnRestart();
        }
        
        protected override void OnSaveInstanceState(Bundle outState)
        {
            Console.WriteLine("[QR2WEB Task] - SAVE INSTANCE");
            _myWebView?.SaveView(outState);
            base.OnSaveInstanceState(outState);
        }

        public override void OnSaveInstanceState(Bundle outState, PersistableBundle outPersistentState)
        {
            Console.WriteLine("[QR2WEB Task] - SAVE INSTANCE 2");
            _myWebView?.SaveView(outState);
            base.OnSaveInstanceState(outState, outPersistentState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            Console.WriteLine("[QR2WEB Task] - RESTORE INSTANCE");
            _myWebView?.RestoreView(savedInstanceState);
            base.OnRestoreInstanceState(savedInstanceState);
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
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
                        });
                    }).Start();
                }
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Console.WriteLine("[QR2WEB Task] - NEW INTENT");

            if (intent.Data != null)
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

            Console.WriteLine("[QR2WEB Task] - START");

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

        private bool MainActivity_BackPressed(object sender, EventArgs e)
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(null))
                return true;
            return App.InvokeBack();
        }

        // obsolete since android 13
        /*
        public override void OnBackPressed()
        {
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }*/

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

