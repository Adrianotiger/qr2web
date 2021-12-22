using System;
using System.Collections.Generic;
using System.Text;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace QR2Web
{
    public class History
    {
        private List<KeyValuePair<DateTime, string>> ScanData = new List<KeyValuePair<DateTime, string>>(16);

        public History()
        {
            Parameters.LoadHistory(ref ScanData);            // load scan results history

            if (!Parameters.Options.SaveHistory) ScanData.Clear();   // clear history if no history should be used
        }

        /// <summary>
        /// Add the QR-code to history. So it can be used later.
        /// </summary>
        /// <param name="value">The QR code or the parsed value (can be text)</param>
        public void Add(string value)
        {
            for (int i = 0; i < ScanData.Count; i++)
            {
                if (ScanData[i].Value == value)
                {
                    if (i == 0) return;
                    ScanData.RemoveAt(i);
                }
            }
            if (Parameters.Options.SaveHistory)
            {
                ScanData.Insert(
                        0,
                        new KeyValuePair<DateTime, string>
                        (
                            DateTime.Now,
                            value
                        )
                    );

                Parameters.SaveHistory(ref ScanData);
            }
        }

        public async void ShowDialog(QRMainPage BasePage)
        {
            var historyPage = new HistoryPage(ScanData);
            await BasePage.Navigation.PushPopupAsync(historyPage);
        }
    }

    class HistoryPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public HistoryPage(List<KeyValuePair<DateTime, string>> ScanData)
        {
            Title = "History";

            var content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = {
                }
            };

            if (ScanData.Count == 0)
            {
                content.Children.Add(new Label { Text = "[empty]", HorizontalTextAlignment = TextAlignment.Center });
            }

            for (int i = 0; i < ScanData.Count; i++)
            {
                Label b = new Label { Text = ScanData[i].Value, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start, BackgroundColor = Color.Transparent, TextColor = Color.SkyBlue, Padding = new Thickness(10, 2, 10, 0) };
                if (ScanData[i].Value.Length < 20) b.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                else if (ScanData[i].Value.Length < 20) b.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                Label l = null;
                if (ScanData[i].Key.Day == DateTime.Now.Day)
                {
                    l = new Label { Text = ScanData[i].Key.ToString("H:mm:ss"), HorizontalOptions = LayoutOptions.EndAndExpand, TextColor = Color.SlateGray, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
                }
                else if (ScanData[i].Key.Day == DateTime.Now.AddDays(-1).Day)
                {
                    l = new Label { Text = ScanData[i].Key.ToString("d.MMM H:mm"), HorizontalOptions = LayoutOptions.EndAndExpand, TextColor = Color.SlateGray, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
                }
                else
                {
                    l = new Label { Text = ScanData[i].Key.ToString("d.MMMM.yyyy"), HorizontalOptions = LayoutOptions.EndAndExpand, TextColor = Color.SlateGray, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
                }

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    var t = (s as Label).Text;
                    App.Instance.InjectJSQRCode(t);
                    Navigation.PopPopupAsync();
                };
                b.GestureRecognizers.Add(tapGestureRecognizer);

                content.Children.Add(b);
                content.Children.Add(l);
            }

            Content = new ScrollView
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 300,
                HeightRequest = 500,
                Padding = new Thickness(10, 5, 10, 5),
                BackgroundColor = Color.Black,
                Content = content
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
    }
}
