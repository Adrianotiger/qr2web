using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QR2Web
{
    class QRMainPage : NavigationPage
    {
        private Button ScanButton;
        private Button HistoryButton;
        private Button MoreButton;
        private Button HomeButton;
        private Button RefreshButton;
        private Button SettingsButton;
        private WebView WebPageWebView;
        private StackLayout TitleStack;
        private DateTime lastWindowUpdate;

        public QRMainPage()
        {

            InitButtons();

            // Don't show button if there is no QR-history 
            HistoryButton.IsVisible = Parameters.Options.SaveHistory;

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

                Console.WriteLine(url);
                if (url.StartsWith("http"))// http or https
                {
                    InjectJS("window.QR2WEB=1");
                }
            };

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
                    var uri = new Uri(url);
                    await Launcher.TryOpenAsync(uri);
                    e.Cancel = true;
                }
                else // http or https
                {

                }
            };
            WebPageWebView.SizeChanged += (s, e) =>
            {
                if (TitleStack.Children.Count == 1)
                {
                    lastWindowUpdate = DateTime.Now;
                }
                else
                {
                    if ((DateTime.Now - lastWindowUpdate).TotalSeconds > 2)
                    {
                        while (TitleStack.Children.Count > 1)
                        {
                            TitleStack.Children.RemoveAt(1);
                        }
                        lastWindowUpdate = DateTime.Now;
                    }
                }

                if (WebPageWebView.Bounds.Width > 200)
                    TitleStack.Children.Add(ScanButton);
                if (WebPageWebView.Bounds.Width > 400)
                    TitleStack.Children.Add(HomeButton);
                if (WebPageWebView.Bounds.Width > 500)
                    TitleStack.Children.Add(RefreshButton);
                if (WebPageWebView.Bounds.Width > 300)
                    TitleStack.Children.Add(HistoryButton);
                if (WebPageWebView.Bounds.Width > 600)
                    TitleStack.Children.Add(SettingsButton);
                TitleStack.Children.Add(MoreButton);

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
                                    TextColor = Color.Yellow
                                }
                            }
            };

            // The root page of your application
            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Spacing = 0,
                Children = {
                        TitleStack,
                        WebPageWebView
                    }
            };
            var page0 = new ContentPage { Content = content };
            SetHasNavigationBar(page0, false);
            SetHasBackButton(page0, App.HasBackButton);

            if (App.HasBackButton)
            {
                var page1 = new ContentPage {  };
                SetHasNavigationBar(page1, false);
                SetHasBackButton(page1, true);
                Navigation.PushAsync(page1);
            }

            Navigation.PushAsync(page0);

            Padding = new Thickness(0);
            BackgroundColor = Color.Black;
        }

        protected override bool OnBackButtonPressed()
        {
            if(Navigation.NavigationStack.Count > 2)
            {
                Navigation.PopAsync();
            }
            if (WebPageWebView.CanGoBack)
            {
                WebPageWebView.GoBack();
            }

            if (App.HasBackButton) return true;

            return base.OnBackButtonPressed();
        }

        private void InitButtons()
        {
            // create top-bar buttons
            ScanButton = new Button
            {
                ImageSource = new FileImageSource
                {
                    File = "scanb.png",
                },
            };
            ScanButton.Clicked += (sender, e) =>
            {
                App.Instance.StartScan();
            };

            HistoryButton = new Button
            {
                ImageSource = new FileImageSource
                {
                    File = "scanh.png",
                },
            };
            HistoryButton.Clicked += HistoryScan_Clicked;

            HomeButton = new Button
            {
                ImageSource = new FileImageSource
                {
                    File = "scanp.png",
                },
            };
            HomeButton.Clicked += GoToHomePage;

            RefreshButton = new Button
            {
                ImageSource = new FileImageSource
                {
                    File = "scanr.png",
                },
            };
            RefreshButton.Clicked += RefreshWebPage;

            SettingsButton = new Button
            {
                ImageSource = new FileImageSource
                {
                    File = "scanc.png",
                },
            };
            SettingsButton.Clicked += async (sender, e) =>
            {
                var optionsPage = new OptionsPage { BackgroundColor = Color.White };
                SetHasNavigationBar(optionsPage, false);
                SetHasBackButton(optionsPage, App.HasBackButton);
                await Navigation.PushAsync(optionsPage);
                //await App.Current.MainPage.Navigation.PushModalAsync(new OptionsPage());
            };

            MoreButton = new Button
            {
                ImageSource = new FileImageSource
                {
                    File = "scanm.png",
                },
            };
            MoreButton.Clicked += MoreScan_Clicked;
        }


        /// <summary>
        /// More button was pressed. A list of available action will be showed. Once pressed the attached action will be executed.
        /// </summary>
        /// <param name="sender">Who called this function</param>
        /// <param name="e">Event-Arguments for the Click-Event</param>
        /// <remarks>
        /// </remarks>
        private async void MoreScan_Clicked(object sender, EventArgs e)
        {
            string[] buttons = { Language.GetText("RefreshPage"), Language.GetText("HomePage"), Language.GetText("Settings"), Language.GetText("Help"), Language.GetText("About") };

            string result = await DisplayActionSheet(Language.GetText("Options"), Language.GetText("Cancel"), null, buttons);

            if (buttons.Contains(result))
            {
                if (result.CompareTo(Language.GetText("Settings")) == 0)
                {
                    await Navigation.PushModalAsync(new OptionsPage());
                }
                else if (result.CompareTo(Language.GetText("RefreshPage")) == 0)
                {
                    RefreshWebPage(sender, e);
                }
                else if (result.CompareTo(Language.GetText("HomePage")) == 0)
                {
                    GoToHomePage(sender, e);
                }
                else if (result.CompareTo(Language.GetText("Help")) == 0)
                {
                    SetSource("https://adrianotiger.github.io/qr2web/help.html");
                }
                else if (result.CompareTo(Language.GetText("About")) == 0)
                {
                    SetSource("https://adrianotiger.github.io/qr2web/info.html?version=" + App.AppVersion.ToString() + "&os=" + Device.RuntimePlatform.ToString());
                }

            }
        }

        /// <summary>
        /// History button was pressed. The last parsed QR-codes will be listed.
        /// </summary>
        /// <param name="sender">Who called this function</param>
        /// <param name="e">Event-Arguments for the Click-Event</param>
        /// <remarks>
        /// Helpfull if you was offline and hadn't access to the webpage.
        /// Time are displayed. If the history is older than 1 day, the day will be displayed.
        /// </remarks>
        private async void HistoryScan_Clicked(object sender, EventArgs e)
        {
            string[] buttons = new string[Math.Max(1, App.History.Count)];

            if (App.History.Count == 0)
            {
                buttons[0] = "empty";
            }

            for (int i = 0; i < App.History.Count; i++)
            {
                if (App.History[i].Key.Day == DateTime.Now.Day)
                    buttons[i] = App.History[i].Value + " (" + App.History[i].Key.ToString("H:mm:ss") + ")";
                else if (App.History[i].Key.Day == DateTime.Now.AddDays(-1).Day)
                    buttons[i] = App.History[i].Value + " (" + App.History[i].Key.ToString("d.MMM H:mm") + ")";
                else
                    buttons[i] = App.History[i].Value + " (" + App.History[i].Key.ToString("d.MMMM.yyyy") + ")";
            }

            string result = await DisplayActionSheet(Language.GetText("History"), Language.GetText("Cancel"), null, buttons);

            if (result.IndexOf("(") > 0)
            {
                App.Instance.OpenJSFunctionQRCode(result.Substring(0, result.IndexOf("(") - 1));
            }
        }

        public void GoToHomePage(object sender, EventArgs e)
        {
            SetSource(Parameters.Options.HomePage);
        }

        public void RefreshWebPage(object sender, EventArgs e)
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
