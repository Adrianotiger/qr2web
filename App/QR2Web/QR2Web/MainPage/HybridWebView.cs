using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QR2Web
{
    public class HybridWebView : WebView
    {
        private Action<string> action;
        private bool firstPageLoaded;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public HybridWebView()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            Margin = new Thickness(0);
            BackgroundColor = Color.White;

            firstPageLoaded = false;

            Navigated += HybridWebView_Navigated;
        }

        public void Initialize()
        {
            Navigating += HybridWebView_Navigating;
        }

        private async void HybridWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            var url = e.Url;
            // catch the pressed link. If link is a valid app protocol (qr2web, barcodereader, ...) start QR scanner
            if (Scanner.IsAppURL(url))
            {
                e.Cancel = true;
                App.StartScanFromWeb(url.ToString());
            }
            else if (url.StartsWith("googlechrome"))
            {
                String url2 = url.Replace("googlechrome", "http");
                await Launcher.TryOpenAsync(url2);
                e.Cancel = true;
            }
            else if (!url.StartsWith("http")) 
            {
                App.IOS.OpenExternalUrl(url);
                e.Cancel = true;
            }
            else // http or https
            {

            }
        }

        private void HybridWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var url = e.Url;
            firstPageLoaded = true;

            Console.WriteLine("Navigated to:" + url);
            if (url.StartsWith("http"))// http or https
            {
                InjectJS("window.QR2WEB=1");
            }
        }

        public bool IsReady()
        {
            return firstPageLoaded;
        }

        public void InjectJS(string js)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    _ = await EvaluateJavaScriptAsync(js);
                }
                catch (Exception e)
                {
                    // BarcodeScanner not available
                    Console.WriteLine(e);
                }
            });
        }

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }
        
    }
}
