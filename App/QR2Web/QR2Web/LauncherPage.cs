using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QR2Web
{
    class LauncherPage : NavigationPage
    {
        public LauncherPage()
        {
            // Splash screen
            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                Children = {
                        new Label
                        {
                            TextColor = Color.SkyBlue,
                            Text = "QR 2 Web",
                            VerticalTextAlignment = TextAlignment.End,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontSize = 30,
                            VerticalOptions = LayoutOptions.FillAndExpand
                        },
                        new Label
                        {
                            TextColor = Color.LightBlue,
                            Text = "By Adriano",
                            VerticalTextAlignment = TextAlignment.Start,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontSize = 10,
                            Margin = new Thickness(10),
                            VerticalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            TextColor = Color.LightBlue,
                            Text = "Version " + VersionTracking.CurrentVersion,
                            VerticalTextAlignment = TextAlignment.Start,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontSize = 10,
                            Margin = new Thickness(10),
                            VerticalOptions = LayoutOptions.FillAndExpand
                        }
                    }
            };

            var page0 = new ContentPage { Content = content };
            SetHasNavigationBar(page0, false);
            SetHasBackButton(page0, false);

            page0.Appearing += Page0_Appearing;

            Navigation.PushAsync(page0);
        }

        private void Page0_Appearing(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(20);
                App.Instance.LoadMainPage(true);
                await Task.Delay(100);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.Instance.StartMainPage();
                });
                await Task.Delay(1000);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.Instance.LoadMainPage(false);
                });
            });
        }

        public void AddPage(Page p)
        {
            MainThread.BeginInvokeOnMainThread(async() =>
            {
                SetHasNavigationBar(p, false);
                SetHasBackButton(p, App.HasBackButton);
                await Navigation.PushAsync(p);
            });
        }

        protected override bool OnBackButtonPressed()
        {
            if (Navigation.NavigationStack.Count > 2)
            {
                return base.OnBackButtonPressed();
            }
            else if(App.Instance.GoBack())
            {
                return true;
            }

            if (App.HasBackButton)
            {
                return true;
            }
            else
            {
                Navigation.PopToRootAsync();
                return base.OnBackButtonPressed();
            }
        }
    }
}
