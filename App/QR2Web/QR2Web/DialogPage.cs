using AndroidX.AppCompat.Widget;
using Google.Android.Material.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace QR2Web
{
    internal class DialogPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public DialogPage(string text, Button b1, Button b2)
        {
            StackLayout baseLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                }
            };

            baseLayout.SetAppThemeColor(Label.BackgroundColorProperty, Color.White, Color.Black);

            var texts = text.Split("\n");
            texts.ForEach(t =>
            {
                baseLayout.Children.Add(new Label { 
                    Text = t, 
                    HorizontalTextAlignment = TextAlignment.Center 
                });
            });

            StackLayout buttonsLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = 
                { 
                }
            };

            if(b1 != null) 
            {
                buttonsLayout.Children.Add(b1);
            }
            if (b2 != null)
            {
                buttonsLayout.Children.Add(b2);
            }
            baseLayout.Children.Add(buttonsLayout);

            Content = baseLayout;
            NavigationPage.SetHasBackButton(this, false);
            
        }

        private void B1_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
