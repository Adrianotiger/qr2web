using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;

namespace QR2Web
{
    class Buttons
    {
        public Button ScanButton = null;
        public Button HistoryButton = null;
        public Button MoreButton = null;
        public Button HomeButton = null;
        public Button RefreshButton = null;
        public Button SettingsButton = null;
        public Button AboutButton = null;
        public Button HelpButton = null;

        private bool mainPageButtons;
        private DateTime lastWindowUpdate;

        private QRMainPage BasePage;

        public Buttons(QRMainPage page)
        {
            BasePage = page;
        }

        private Button CreateButton(string filename)
        {
            return new Button
            {
                ImageSource = new FileImageSource { File = filename },
                Opacity = mainPageButtons ? 0.0 : 1.0,
                BackgroundColor = Color.Transparent,
                WidthRequest = 64
            };
        }

        public void InitButtons(bool fromMainPage = true)
        {
            mainPageButtons = fromMainPage;
            string[] icons = { "scanb.png", "scanh.png", "scanp.png", "scanr.png", "scanc.png", "scanm.png", "help.png", "about.png" };
            int buttIndex = 0;
            // create top-bar buttons
            ScanButton = CreateButton(icons[buttIndex++]);
            ScanButton.Clicked += ScanButton_Clicked;

            HistoryButton = CreateButton(icons[buttIndex++]);
            HistoryButton.Clicked += HistoryScan_Clicked;

            HomeButton = CreateButton(icons[buttIndex++]);
            HomeButton.Clicked += HomeButton_Clicked;

            RefreshButton = CreateButton(icons[buttIndex++]);
            RefreshButton.Clicked += RefreshButton_Clicked;

            SettingsButton = CreateButton(icons[buttIndex++]);
            SettingsButton.Clicked += (sender, e) =>
            {
                var optionsPage = new OptionsPage { BackgroundColor = Color.White };
                App.Instance.NavigateTo(optionsPage);
            };

            MoreButton = CreateButton(icons[buttIndex++]);
            MoreButton.Clicked += MoreScan_Clicked;

            HelpButton = CreateButton(icons[buttIndex++]);
            HelpButton.Clicked += HelpButton_Clicked;

            AboutButton = CreateButton(icons[buttIndex++]);
            AboutButton.Clicked += AboutButton_Clicked;

            /////////////////////////////

            // Don't show button if there is no QR-history 
            HistoryButton.IsVisible = Parameters.Options.SaveHistory;
        }

        private void AboutButton_Clicked(object sender, EventArgs e)
        {
            BasePage.SetSource("https://adrianotiger.github.io/qr2web/info.html?version=" + App.AppVersion.ToString() + "&os=" + Device.RuntimePlatform.ToString());
        }

        private void HelpButton_Clicked(object sender, EventArgs e)
        {
            BasePage.SetSource("https://adrianotiger.github.io/qr2web/help.html");
        }

        private void AniButton(Button b, int delay)
        {
            Task.Run(async () =>
            {
                await Task.Delay(delay);
                MainThread.BeginInvokeOnMainThread(() => { b.FadeTo(1.0, 450); });
            });
        }

        public void UpdateIcons(StackLayout stack, double layoutWidth)
        {
            if (stack.Children.Count == 1)
            {
                lastWindowUpdate = DateTime.Now;
            }
            else
            {
                if ((DateTime.Now - lastWindowUpdate).TotalSeconds > 2)
                {
                    while (stack.Children.Count > 1)
                    {
                        stack.Children.RemoveAt(1);
                    }
                    lastWindowUpdate = DateTime.Now;
                }
            }

            int timeout = 300;
            if (layoutWidth > 200)
            {
                AniButton(ScanButton, timeout);
                timeout += 200;
                stack.Children.Add(ScanButton);
            }
            if (layoutWidth > 400)
            {
                AniButton(HomeButton, timeout);
                timeout += 200;
                stack.Children.Add(HomeButton);
            }
            if (layoutWidth > 500)
            {
                AniButton(RefreshButton, timeout);
                timeout += 200;
                stack.Children.Add(RefreshButton);
            }
            if (layoutWidth > 300)
            {
                AniButton(HistoryButton, timeout);
                timeout += 200;
                stack.Children.Add(HistoryButton);
            }
            if (layoutWidth > 600)
            {
                AniButton(SettingsButton, timeout);
                timeout += 200;
                stack.Children.Add(SettingsButton);
            }
            AniButton(MoreButton, timeout);
            stack.Children.Add(MoreButton);
        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            App.Instance.StartScan();
        }

        private void RefreshButton_Clicked(object sender, EventArgs e)
        {
            BasePage.RefreshWebPage();
        }

        private void HomeButton_Clicked(object sender, EventArgs e)
        {
            BasePage.GoToHomePage();
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
            var menuPage = new MenuPage(BasePage);
            await BasePage.Navigation.PushPopupAsync(menuPage);
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
        private void HistoryScan_Clicked(object sender, EventArgs e)
        {
            App.Instance.ScanHistory.ShowDialog(BasePage);
        }

    }
}
