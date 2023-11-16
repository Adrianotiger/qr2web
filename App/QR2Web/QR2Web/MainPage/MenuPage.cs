using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;

namespace QR2Web
{
    public class MenuPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private readonly Buttons NavButtons;

        public MenuPage(QRMainPage mainPage)
        {
            NavButtons = new Buttons(mainPage);

            Title = "Options";
            BackgroundColor = Color.FromRgba(0.4, 0.4, 0.4, 0.4);

            Content = new Grid
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 200,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(10, 43, 10, 0),
                BackgroundColor = Color.Black,
                RowDefinitions = { new RowDefinition { Height = 90 }, new RowDefinition { Height = 90 } },
                ColumnDefinitions = { new ColumnDefinition { Width = 80 }, new ColumnDefinition { Width = 80 }, new ColumnDefinition { Width = 80 }, new ColumnDefinition { Width = 80 } },
                Children = {
                }
            };

            NavButtons.InitButtons(false);
        }

        private void AddButton(Button butt, string text, int col, int row)
        {
            var grid = Content as Grid;
            butt.Margin = new Thickness(0);
            grid.Children.Add(
                new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Spacing = 0,
                    Padding = new Thickness(0),
                    Children =
                    {
                        butt,
                        new Label { Text = text, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), TextColor  = Color.White, HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0) }
                    }
                }
                , col, row
            );
            butt.Clicked += (s, e) =>
            {
                Navigation.PopPopupAsync();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AddButton(NavButtons.ScanButton, Language.GetText("Scan"), 0, 0);
            AddButton(NavButtons.HistoryButton, Language.GetText("History"), 1, 0);
            AddButton(NavButtons.SettingsButton, Language.GetText("Settings"), 3, 0);

            AddButton(NavButtons.HomeButton, Language.GetText("Home"), 0, 1);
            AddButton(NavButtons.RefreshButton, Language.GetText("RefreshPage"), 1, 1);
            AddButton(NavButtons.HelpButton, Language.GetText("Help"), 2, 1);
            AddButton(NavButtons.AboutButton, Language.GetText("About"), 3, 1);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            //Navigation.PopToRootAsync();
            //return true;
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
