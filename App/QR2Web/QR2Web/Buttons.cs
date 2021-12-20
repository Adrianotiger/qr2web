using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Linq;
using System.Threading.Tasks;

namespace QR2Web
{
    class Buttons
    {
        private Button ScanButton;
        private Button HistoryButton;
        private Button MoreButton;
        private Button HomeButton;
        private Button RefreshButton;
        private Button SettingsButton;
        
        private DateTime lastWindowUpdate;

        private QRMainPage BasePage;

        public Buttons(QRMainPage page)
        {
            BasePage = page;
        }

        public void InitButtons()
        {
            string[] icons = { "scanb.png", "scanh.png", "scanp.png", "scanr.png", "scanc.png", "scanm.png" };
            int buttIndex = 0;
            // create top-bar buttons
            ScanButton = new Button
            {
                ImageSource = new FileImageSource{File = icons[buttIndex++]},
                Opacity = 0.0
            };
            ScanButton.Clicked += ScanButton_Clicked;

            HistoryButton = new Button
            {
                ImageSource = new FileImageSource { File = icons[buttIndex++] },
                Opacity = 0.0
            };
            HistoryButton.Clicked += HistoryScan_Clicked;

            HomeButton = new Button
            {
                ImageSource = new FileImageSource { File = icons[buttIndex++] },
                Opacity = 0.0
            };
            HomeButton.Clicked += HomeButton_Clicked;

            RefreshButton = new Button
            {
                ImageSource = new FileImageSource { File = icons[buttIndex++] },
                Opacity = 0.0
            };
            RefreshButton.Clicked += RefreshButton_Clicked;

            SettingsButton = new Button
            {
                ImageSource = new FileImageSource { File = icons[buttIndex++] },
                Opacity = 0.0
            };
            SettingsButton.Clicked += (sender, e) =>
            {
                var optionsPage = new OptionsPage { BackgroundColor = Color.White };
                App.Instance.NavigateTo(optionsPage);
            };

            MoreButton = new Button
            {
                ImageSource = new FileImageSource { File = icons[buttIndex++] },
                Opacity = 0.0
            };
            MoreButton.Clicked += MoreScan_Clicked;

            /////////////////////////////

            // Don't show button if there is no QR-history 
            HistoryButton.IsVisible = Parameters.Options.SaveHistory;
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
            string[] buttons = { Language.GetText("RefreshPage"), Language.GetText("HomePage"), Language.GetText("Settings"), Language.GetText("Help"), Language.GetText("About") };

            string result = await BasePage.DisplayActionSheet(Language.GetText("Options"), Language.GetText("Cancel"), null, buttons);

            if (buttons.Contains(result))
            {
                if (result.CompareTo(Language.GetText("Settings")) == 0)
                {
                    App.Instance.NavigateTo(new OptionsPage());
                }
                else if (result.CompareTo(Language.GetText("RefreshPage")) == 0)
                {
                    BasePage.RefreshWebPage();
                }
                else if (result.CompareTo(Language.GetText("HomePage")) == 0)
                {
                    BasePage.GoToHomePage();
                }
                else if (result.CompareTo(Language.GetText("Help")) == 0)
                {
                    BasePage.SetSource("https://adrianotiger.github.io/qr2web/help.html");
                }
                else if (result.CompareTo(Language.GetText("About")) == 0)
                {
                    BasePage.SetSource("https://adrianotiger.github.io/qr2web/info.html?version=" + App.AppVersion.ToString() + "&os=" + Device.RuntimePlatform.ToString());
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

            string result = await BasePage.DisplayActionSheet(Language.GetText("History"), Language.GetText("Cancel"), null, buttons);

            if (result.IndexOf("(") > 0)
            {
                App.Instance.OpenJSFunctionQRCode(result.Substring(0, result.IndexOf("(") - 1));
            }
        }

    }
}
