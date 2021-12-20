using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QR2Web
{
    class QRMainPage : ContentPage
    {
        private Buttons NavButtons;
        private WebView WebPageWebView;
        private StackLayout TitleStack;
        private bool firstPageLoaded = false;

        public QRMainPage()
        {
            NavButtons = new Buttons(this);

            WebPageWebView = new WebView
            {
                Source = Parameters.Options.HomePage,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0),
                BackgroundColor = Color.White,
            };

            WebPageWebView.Navigated += (s, e) =>
            {
                var url = e.Url;
                firstPageLoaded = true;

                Console.WriteLine(url);
                if (url.StartsWith("http"))// http or https
                {
                    InjectJS("window.QR2WEB=1");
                }
            };

            TitleStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 45,
                Padding = new Thickness(5, 0, 5, 0),
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.Black,
                Children = {
                    new Label
                    {
                        Text = "QR 2 Web",
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.Fill,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.SkyBlue
                    }
                }
            };

            // The root page of your application
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
                Children = {
                        TitleStack,
                        WebPageWebView
                    }
            };

            Padding = new Thickness(0);
            BackgroundColor = Color.Black;
        }

        public void Initialize()
        {
            NavButtons.InitButtons();

            WebPageWebView.Navigating += async (sender, /*WebNavigatingEventArgs*/ e) =>
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
            };

            WebPageWebView.SizeChanged += (s, e) =>
            {
                NavButtons.UpdateIcons(TitleStack, WebPageWebView.Bounds.Width);
            };

            NavButtons.UpdateIcons(TitleStack, WebPageWebView.Bounds.Width);
        }

        public bool IsLoaded()
        {
            return firstPageLoaded;
        }

        public bool Back()
        {
            if (WebPageWebView.CanGoBack)
            {
                WebPageWebView.GoBack();
                return true;
            }
            return false;
        }

        public void GoToHomePage()
        {
            SetSource(Parameters.Options.HomePage);
        }

        public void RefreshWebPage()
        {
            if (Device.RuntimePlatform == Device.UWP)
                SetSource((WebPageWebView.Source as UrlWebViewSource).Url);
            else
                WebPageWebView.Reload();
        }

        public void SetSource(string url)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                WebPageWebView.Source = url;
            });
        }

        public void InjectJS(string js)
        {
            Device.BeginInvokeOnMainThread(async() =>
            {
                try
                {
                    await WebPageWebView.EvaluateJavaScriptAsync(js);
                    //WebPageWebView.Eval(js);
                }
                catch (Exception e)
                {
                    // BarcodeScanner not available
                    Console.WriteLine(e);
                }
            });
        }

    }
}
