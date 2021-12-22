using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QR2Web
{
    public class QRMainPage : ContentPage
    {
        private Buttons NavButtons;
        private HybridWebView WebPageWebView;
        private StackLayout TitleStack;

        public QRMainPage()
        {
            NavButtons = new Buttons(this);

            WebPageWebView = new HybridWebView
            {
                Source = Parameters.Options.HomePage
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
                        Text = Language.GetText("AppTitleShort"),
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
            WebPageWebView.Initialize();

            WebPageWebView.SizeChanged += (s, e) =>
            {
                NavButtons.UpdateIcons(TitleStack, WebPageWebView.Bounds.Width);
            };

            NavButtons.UpdateIcons(TitleStack, WebPageWebView.Bounds.Width);
        }

        public bool IsLoaded()
        {
            return WebPageWebView.IsReady();
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

        public void InjectJS(String javascript)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                WebPageWebView.InjectJS(javascript);
            });
        }

    }
}
