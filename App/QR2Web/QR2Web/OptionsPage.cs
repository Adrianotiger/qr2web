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
		private bool saveSettings = false;							// if settings should be saved once the page is closed

		private Entry webPageValue;									// home page option
		private Picker emulationPicker;								// emulation option
		private Switch[] acceptedCodesSwitch = new Switch[4];		// accepted codes option
		private Label[] acceptedCodesTexts = new Label[4];			// texts for the accepted codes
		private Switch lockPortraitSwitch;							// lock portrait mode option
		private Switch saveHistorySwitch;							// save history option
		private Picker languagePicker;								// language option

		/// <summary>
		/// Constructor. Initialize the page view and the components.
		/// </summary>
		public OptionsPage()
		{
			Label optionTitle = new Label { Text = Language.GetText("Settings"), Scale = 3.0 };

			Label webPageText = new Label { Text = Language.GetText("Option1_1") };
			webPageValue = new Entry { Text = Parameters.Options.HomePage };
			webPageValue.TextChanged += (s, e) => { saveSettings = true; };

			Label emulationModeText = new Label { Text = Language.GetText("Option2_1") };
			emulationPicker = new Picker { Title = Language.GetText("Option2_1") };
			emulationPicker.Items.Add(Language.GetText("Option2_2"));
			emulationPicker.Items.Add("MochaSoft");
			emulationPicker.Items.Add("Pic2Shop");
			emulationPicker.SelectedIndex = Parameters.Options.Emulation;
			emulationPicker.SelectedIndexChanged += (s, e) => { saveSettings = true; };

			Label acceptedCodesText = new Label { Text = Language.GetText("Option3_1") };
			acceptedCodesTexts[0] = new Label { Text = "QR", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
			acceptedCodesSwitch[0] = new Switch { IsToggled = true, IsEnabled = false };
			acceptedCodesTexts[1] = new Label { Text = "CODE", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
			acceptedCodesSwitch[1] = new Switch { IsToggled = Parameters.Options.AcceptBarcode_Code, IsEnabled = true };
			acceptedCodesTexts[2] = new Label { Text = "EAN", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
			acceptedCodesSwitch[2] = new Switch { IsToggled = Parameters.Options.AcceptBarcode_Ean, IsEnabled = true };
			acceptedCodesTexts[3] = new Label { Text = "UPC", WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center };
			acceptedCodesSwitch[3] = new Switch { IsToggled = Parameters.Options.AcceptBarcode_Upc, IsEnabled = true };

			for(int i=0;i<4;i++)
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
			languagePicker = new Picker { Title = Language.GetText("Option6_1") };
			languagePicker.Items.Add(Language.GetText("Option6_2"));
			languagePicker.Items.Add(Language.GetText("Option6_3"));
			languagePicker.Items.Add(Language.GetText("Option6_4"));
			languagePicker.SelectedIndex = Parameters.Options.LanguageIndex;
			languagePicker.SelectedIndexChanged += (s, e) => { saveSettings = true; };
					


			Label NeedRestartText = new Label { Text = "--------------------------\n" + Language.GetText("NeedRestart") };


			Content = new StackLayout
			{
				Padding = new Thickness(10),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					optionTitle,
					webPageText,
					webPageValue,
					acceptedCodesText,
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							acceptedCodesTexts[0],
							acceptedCodesSwitch[0]
						}
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							acceptedCodesTexts[1],
							acceptedCodesSwitch[1]
						}
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							acceptedCodesTexts[2],
							acceptedCodesSwitch[2]
						}
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							acceptedCodesTexts[3],
							acceptedCodesSwitch[3]
						}
					},
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
				Parameters.Options.HomePage = webPageValue.Text;
				Parameters.Options.Emulation = emulationPicker.SelectedIndex;
				Parameters.Options.AcceptBarcode_Code = acceptedCodesSwitch[1].IsToggled;
				Parameters.Options.AcceptBarcode_Ean = acceptedCodesSwitch[2].IsToggled;
				Parameters.Options.AcceptBarcode_Upc = acceptedCodesSwitch[3].IsToggled;
				Parameters.Options.LockPortrait = lockPortraitSwitch.IsToggled;
				Parameters.Options.SaveHistory = saveHistorySwitch.IsToggled;
				Parameters.Options.LanguageIndex = languagePicker.SelectedIndex;
				Parameters.SaveParams();
			}

			this.Navigation.PopModalAsync();	// close this page and return the the preview page (main page)
			return true;
		}
	}
}
