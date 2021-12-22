using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace QR2Web
{
    /// <summary>
    /// Option Page to show all options of this app.
    /// </summary>
    public class OptionsPage : ContentPage
    {
        private bool saveSettings = false;                          // if settings should be saved once the page is closed

        private Entry webPageValue;                                 // home page option
        private Picker emulationPicker;                             // emulation option
        private Switch[] acceptedCodesSwitch = new Switch[4];       // accepted codes option
        private Label[] acceptedCodesTexts = new Label[4];          // texts for the accepted codes
        private Label[] acceptedCodesDesc = new Label[4];          // texts for the accepted codes
        private Switch sendLocationSwitch;                          // location activation option
        private Switch lockPortraitSwitch;                          // lock portrait mode option
        private Switch saveHistorySwitch;                           // save history option
        private Picker languagePicker;                              // language option

        /// <summary>
        /// Constructor. Initialize the page view and the components.
        /// </summary>
        public OptionsPage()
        {
            Label webPageText = new Label { Text = Language.GetText("Option1_1") };
            webPageValue = new Entry { Text = Parameters.Options.HomePage };
            webPageValue.TextChanged += (s, e) => { saveSettings = true; };

            Label emulationModeText = new Label { Text = Language.GetText("Option2_1") };
            emulationPicker = new Picker { HorizontalOptions = LayoutOptions.End };
            emulationPicker.Items.Add(Language.GetText("Option2_2"));
            emulationPicker.Items.Add("MochaSoft");
            emulationPicker.Items.Add("Pic2Shop");
            emulationPicker.SelectedIndex = Parameters.Options.Emulation;
            emulationPicker.SelectedIndexChanged += (s, e) => { saveSettings = true; };

            Label acceptedCodesText = new Label { Text = Language.GetText("Option3_1") };
            acceptedCodesTexts[0] = new Label { Text = "QR", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
            acceptedCodesSwitch[0] = new Switch { IsToggled = true, IsEnabled = false };
            acceptedCodesDesc[0] = new Label { Text = "QR-Codes", HorizontalTextAlignment = TextAlignment.End };
            acceptedCodesTexts[1] = new Label { Text = "CODE", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
            acceptedCodesSwitch[1] = new Switch { IsToggled = Parameters.Options.AcceptBarcode_Code, IsEnabled = true };
            acceptedCodesDesc[1] = new Label { Text = "Code-39/93/128/bar", HorizontalTextAlignment = TextAlignment.End };
            acceptedCodesTexts[2] = new Label { Text = "EAN", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
            acceptedCodesSwitch[2] = new Switch { IsToggled = Parameters.Options.AcceptBarcode_Ean, IsEnabled = true };
            acceptedCodesDesc[2] = new Label { Text = "Ean 8/13", HorizontalTextAlignment = TextAlignment.End };
            acceptedCodesTexts[3] = new Label { Text = "UPC", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
            acceptedCodesSwitch[3] = new Switch { IsToggled = Parameters.Options.AcceptBarcode_Upc, IsEnabled = true };
            acceptedCodesDesc[3] = new Label { Text = "Upc-A/E/EAN", HorizontalTextAlignment = TextAlignment.End };

            for (int i = 0; i < 4; i++)
            {
                acceptedCodesSwitch[i].Toggled += (s, e) => { saveSettings = true; };
            }

            Label lockPortraitText = new Label { Text = Language.GetText("Option4_1") + " *", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalTextAlignment = TextAlignment.Center };
            lockPortraitSwitch = new Switch { IsToggled = Parameters.Options.LockPortrait, HorizontalOptions = LayoutOptions.End };
            lockPortraitSwitch.Toggled += (s, e) => { saveSettings = true; };

            Label SaveHistoryText = new Label { Text = Language.GetText("Option5_1") + "*", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalTextAlignment = TextAlignment.Center };
            saveHistorySwitch = new Switch { IsToggled = Parameters.Options.SaveHistory, HorizontalOptions = LayoutOptions.End };
            saveHistorySwitch.Toggled += (s, e) => { saveSettings = true; };

            Label languageText = new Label { Text = Language.GetText("Option6_1") + " *" };
            languagePicker = new Picker { HorizontalOptions = LayoutOptions.End };
            languagePicker.Items.Add(Language.GetText("Option6_2"));
            languagePicker.Items.Add(Language.GetText("Option6_3"));
            languagePicker.Items.Add(Language.GetText("Option6_4"));
            languagePicker.SelectedIndex = Parameters.Options.LanguageIndex;
            languagePicker.SelectedIndexChanged += (s, e) => { saveSettings = true; };

            Label sendLocationText = new Label { Text = Language.GetText("Option7_1"), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalTextAlignment = TextAlignment.Center };
            sendLocationSwitch = new Switch { IsToggled = Parameters.Options.UseLocation, HorizontalOptions = LayoutOptions.End };
            sendLocationSwitch.Toggled += (s, e) => {
                if (!QRLocation.IsLocationAvailableOnDevice() && sendLocationSwitch.IsToggled)
                {
                    sendLocationSwitch.IsToggled = false;
                    App.IOS.EnablePermissionLocation();
                    return;
                }
                saveSettings = true; 
            };

            Label NeedRestartText = new Label { Text = "--------------------------\n" + Language.GetText("NeedRestart") };

            StackLayout backbuttonLayout = new StackLayout();
            backbuttonLayout.Orientation = StackOrientation.Horizontal;
            backbuttonLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            Button backbuttonButton = new Button
            {
                Text = " < ",
                TextColor = Color.Blue,
                WidthRequest = 50,
                FontSize = 20,
            };
            backbuttonButton.Clicked += async (s, e) =>
            {
                OnBackButtonPressed();
                var rootPage = new NavigationPage(this);
                await rootPage.PopAsync();
            };
            Label optionTitle = new Label
            {
                Text = Language.GetText("Settings"),
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.Blue
            };

            if (Device.RuntimePlatform == Device.iOS)
                backbuttonLayout.Children.Add(backbuttonButton);
            backbuttonLayout.Children.Add(optionTitle);

            var scrollView = new StackLayout
            {
                Padding = new Thickness(10),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    backbuttonLayout,
                    acceptedCodesText,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            acceptedCodesTexts[0],
                            acceptedCodesSwitch[0],
                            acceptedCodesDesc[0]
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            acceptedCodesTexts[1],
                            acceptedCodesSwitch[1],
                            acceptedCodesDesc[1]
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            acceptedCodesTexts[2],
                            acceptedCodesSwitch[2],
                            acceptedCodesDesc[2]
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            acceptedCodesTexts[3],
                            acceptedCodesSwitch[3],
                            acceptedCodesDesc[3]
                        }
                    },
                    webPageText,
                    webPageValue,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            lockPortraitText,
                            lockPortraitSwitch
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            sendLocationText,
                            sendLocationSwitch
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            SaveHistoryText,
                            saveHistorySwitch
                        }
                    },
                    languageText,
                    languagePicker,
                    emulationModeText,
                    emulationPicker,

                    NeedRestartText
                },
            };

            Content = new ScrollView { Content = scrollView };

        }

        /// <summary>
        /// Back button was pressed. If a option was changed, all parameters will be saved.
        /// </summary>
        /// <returns>true to override the functionality of the default handler.</returns>
        /// <remarks>
        /// This works on Windows Phones and Android with the back buttons.
        /// Under Windows the "back button" must be set as visible (UWP main app).
        /// Under iOS a back button must be placed manually somewhere.
        /// </remarks>
        protected override bool OnBackButtonPressed()
        {
            if (saveSettings)
            {
                if (!Parameters.Options.UseLocation && sendLocationSwitch.IsToggled)
                {
                    QRLocation.InitLocation();
                }
                Parameters.Options.HomePage = webPageValue.Text;
                Parameters.Options.Emulation = emulationPicker.SelectedIndex;
                Parameters.Options.AcceptBarcode_Code = acceptedCodesSwitch[1].IsToggled;
                Parameters.Options.AcceptBarcode_Ean = acceptedCodesSwitch[2].IsToggled;
                Parameters.Options.AcceptBarcode_Upc = acceptedCodesSwitch[3].IsToggled;
                Parameters.Options.LockPortrait = lockPortraitSwitch.IsToggled;
                Parameters.Options.SaveHistory = saveHistorySwitch.IsToggled;
                Parameters.Options.LanguageIndex = languagePicker.SelectedIndex;
                Parameters.Options.UseLocation = sendLocationSwitch.IsToggled;
            }

            return base.OnBackButtonPressed();
            //this.Navigation.PopModalAsync();    // close this page and return the the preview page (main page)
            //return true;
        }
    }
}
