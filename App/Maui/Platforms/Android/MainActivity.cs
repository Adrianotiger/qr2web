using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using Android.Widget;

using qr2web;
using qr2web.Platforms.Android;

namespace Maui
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        AlwaysRetainTaskState = true,
        ResumeWhilePausing = true,
        HardwareAccelerated = true,
        NoHistory = false,
        ClearTaskOnLaunch = false,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation /*| ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density*/
    )]
    [IntentFilter(
        [Intent.ActionView],
        Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
        DataSchemes = ["qr2web"]
    )]
    public class MainActivity : MauiAppCompatActivity
    {
        Intent? serviceIntent = null;
        static MainActivity? Instance = null;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;

            if(Intent != null)
            {
                if (Intent.Action == Intent.ActionView)
                {
                    if (Intent.Scheme != null && Intent.DataString != null)
                    {
                        LaunchScheme(Intent.DataString);
                    }
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (Options.ForcePortrait)
            {
                this.RequestedOrientation = ScreenOrientation.Portrait;
            }
            else
            {
                this.RequestedOrientation = ScreenOrientation.Sensor;
            }

            if (Options.ForceBackground)
            {
                serviceIntent = new Intent(this, typeof(MainService));
                serviceIntent.SetAction("qr2web.START_SERVICE");
                StartForegroundService(serviceIntent);
            }

            Task.Run(async () =>
            {
                var webView = await WaitForBrowser();
                if (webView != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);

                        webView.Settings.SetSupportMultipleWindows(true);
                        webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                        webView.SetWebChromeClient(new MyWebChromeClient());
                    });
                }
            });
        }

        protected override void OnNewIntent(Intent? intent)
        {
            if (intent != null && intent.Action != null)
            {
                if (intent.Action.Equals("qr2web.CLOSE_APP"))
                {
                    if (serviceIntent != null)
                        StopService(serviceIntent);

                    Process.KillProcess(Process.MyPid());
                    return;
                }
                else if(intent.Action == Intent.ActionView)
                {
                    if(intent.Scheme != null && intent.DataString != null)
                    {
                        LaunchScheme(intent.DataString);
                    }
                }

                //var toast = Toast.MakeText(this, "NEW INTENT: " + intent.Action.ToString(), Android.Widget.ToastLength.Long); // in Activity
                //toast?.Show();
            }
            base.OnNewIntent(intent);
        }

        private static async void LaunchScheme(string dataString)
        {
            await Task.Run(async () =>
            {
                await WaitForBrowser();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.InsertCode(dataString);
                });
            });
        }

        private static async Task<Android.Webkit.WebView?> WaitForBrowser()
        {
            await Task.Run(async () =>
            {
                for (int j = 0; j < 50; j++)
                {
                    if (MainPage.MyWebView?.Handler?.PlatformView is not Android.Webkit.WebView)
                    {
                        await Task.Delay(200);
                    }
                }
            });

            return MainPage.MyWebView?.Handler?.PlatformView as Android.Webkit.WebView;
        }

        protected override void OnDestroy()
        {
            if (serviceIntent != null)
            {
                Android.App.Application.Context.StopService(serviceIntent);
            }
            base.OnDestroy();
        }

        public static void OpenExternalLink(string url)
        {
            if (Instance != null)
            {
                Intent intent = new Intent(Intent.ActionView)
                        .SetData(Android.Net.Uri.Parse(url))
                        .SetFlags(ActivityFlags.NewTask);
                Instance.StartActivity(intent);
            }
        }
    }

    public class MyWebChromeClient : WebChromeClient
    {
        public override bool OnCreateWindow(Android.Webkit.WebView? view, bool isDialog, bool isUserGesture, Message? resultMsg)
        {
            var webViewClient = new MyWebViewClient();

            Android.Webkit.WebView newWebView = new(Android.App.Application.Context);
            Android.Webkit.WebView.WebViewTransport? transport = (Android.Webkit.WebView.WebViewTransport?)resultMsg?.Obj;
            if (transport != null)
            {
                newWebView.SetWebViewClient(webViewClient);
                transport.WebView = newWebView;
                resultMsg?.SendToTarget();
            }

            return true;
        }
    }

    public class MyWebViewClient : WebViewClient
    {
        public override WebResourceResponse? ShouldInterceptRequest(Android.Webkit.WebView? view, IWebResourceRequest? request)
        {
            return base.ShouldInterceptRequest(view, request);
        }

        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView? view, IWebResourceRequest? request)
        {
            var url = request?.Url?.ToString();
            if (url != null)
            {
                if (url.StartsWith("http"))
                {
                    
                    MainPage.OpenExternalLink(url);
                    return true;
                }
            }
            return base.ShouldOverrideUrlLoading(view, request);
        }
    }
}
