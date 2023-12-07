using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Java.Interop;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
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
        ExcludeFromRecents = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation /*| ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density*/)]
    public class MainActivity : MauiAppCompatActivity
    {
        Intent? serviceIntent = null;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
                for (int j = 0; j < 30; j++)
                {
                    if (MainPage.MyWebView?.Handler?.PlatformView is not Android.Webkit.WebView webView)
                    {
                        await Task.Delay(500);
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            webView.Settings.SetSupportMultipleWindows(true);
                            webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                            webView.SetWebChromeClient(new MyWebChromeClient());
                        });
                        break;
                    }
                }
            });
        }

        protected override void OnNewIntent(Intent? intent)
        {
            if (intent != null && intent.Action != null && intent.Action.Equals("qr2web.CLOSE_APP"))
            {
                if (serviceIntent != null)
                    StopService(serviceIntent);

                Process.KillProcess(Process.MyPid());
                return;
            }
            base.OnNewIntent(intent);
        }

        protected override void OnDestroy()
        {
            if (serviceIntent != null)
            {
                Android.App.Application.Context.StopService(serviceIntent);
            }
            base.OnDestroy();
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
