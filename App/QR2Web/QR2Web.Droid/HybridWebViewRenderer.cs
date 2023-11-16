using Android.Content;
using Android.OS;
using AndroidX.Lifecycle;
using QR2Web;
using QR2Web.Droid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace QR2Web.Droid
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        //const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        readonly Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                ((HybridWebView)Element).Cleanup();
            }
            if (e.NewElement != null)
            {
                ((MainActivity)_context).SetWebView(this);

                Control.Settings.SetSupportMultipleWindows(true);
                Control.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                Control.SetWebChromeClient(new MyWebViewClient(this));
                //Control.SetWebViewClient(new JavascriptWebViewClient(this, $"javascript: {JavascriptFunction}"));
                //Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                //Control.LoadUrl($"file:///android_asset/Content/{((HybridWebView)Element).Uri}");
            }
        }

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((HybridWebView)Element).Cleanup();
            }
            base.Dispose(disposing);
        }


        public void SaveView(Bundle bundle)
        {
            var res = Control.SaveState(bundle);
            if(res == null)
            {
                Console.WriteLine("[QR2WEB Task] - ERROR ON SAVING");
            }
            else
            {
                Console.WriteLine("[QR2WEB Task] - SUCCESSFULLY SAVED");
            }
        }

        public void RestoreView(Bundle bundle)
        {
            Control.RestoreState(bundle);
        }

    }

    public class MyWebViewClient2 : Android.Webkit.WebViewClient
    {
        readonly HybridWebViewRenderer _hybridRenderer = null;
        Android.Net.Uri _url = null;

        public MyWebViewClient2(HybridWebViewRenderer hybridRenderer) : base()
        {
            _hybridRenderer = hybridRenderer;
        }

        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, Android.Webkit.IWebResourceRequest request)
        {
            _url = request.Url;
            if (_url.ToString().StartsWith("http"))
            {
                //new Task(async () =>
                {
                    var butt = new Button { Text = "Open" };
                    butt.Pressed += (s, e) =>
                    {
                        ((MainActivity)_hybridRenderer.Context).OpenExternalUrl(_url.ToString());
                    };
                    var butt2 = new Button { Text = "Cancel" };

                    App.Instance.ShowDialog("Open external link?\n" + _url.ToString(), butt, butt2);
                }//);
            }

            this.Dispose();
            return true;
        }
    }

    public class MyWebViewClient : Android.Webkit.WebChromeClient
    {
        readonly HybridWebViewRenderer _hybridRenderer = null;

        public MyWebViewClient(HybridWebViewRenderer hybridRenderer) : base()
        {
            _hybridRenderer = hybridRenderer;
            new WebViewRenderer(_hybridRenderer.Context);
        }


        public override bool OnCreateWindow(Android.Webkit.WebView view, bool isDialog, bool isUserGesture, Android.OS.Message resultMsg)
        {
            Android.Webkit.WebView newWebView = new Android.Webkit.WebView(_hybridRenderer.Context);
            newWebView.SetWebViewClient(new MyWebViewClient2(_hybridRenderer));

            Android.Webkit.WebView.WebViewTransport transport = (Android.Webkit.WebView.WebViewTransport)resultMsg.Obj;
            transport.WebView = newWebView;
            resultMsg.SendToTarget();
            return true;
        }

    }
}