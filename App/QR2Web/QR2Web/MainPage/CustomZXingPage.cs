using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using static ZXing.Mobile.MobileBarcodeScanningOptions;

namespace QR2Web
{
    public class CustomScanPage : ContentPage
    {
        ZXingScannerView zxing;
        public ZXing.Result result { get; set; }

        public CustomScanPage() : base()
        {
            result = null;

            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            SetScanOptions();

            zxing.OnScanResult += Zxing_OnScanResult;

            var inText = new Label
            {
                HeightRequest = 32,
                WidthRequest = 120,
                BackgroundColor = Color.White,
                Text = "",
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            string[] texts = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "⬅", "0", "▶" };
            var buttons = new Button[texts.Length];
            for(var j=0;j<texts.Length; j++)
            {
                buttons[j] = new Button { Text = texts[j], WidthRequest = 65, HeightRequest = 65, CornerRadius = 6, FontSize = 28, FontAttributes = FontAttributes.Bold };
                buttons[j].Clicked += (s, e) =>
                {
                    if(int.TryParse((s as Button).Text, out int num))
                    {
                        inText.Text += (s as Button).Text;
                    }
                    else if((s as Button).Text == "⬅")
                    {
                        if (inText.Text.Length > 0) inText.Text = inText.Text.Substring(0, inText.Text.Length - 1);
                    }
                    else
                    {
                        zxing.RaiseScanResult(new ZXing.Result(inText.Text, Encoding.ASCII.GetBytes(inText.Text), null, ZXing.BarcodeFormat.QR_CODE));
                    }
                };
            }
            var customKeyboard = new Grid
            {
                RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = 38 }, new RowDefinition { Height = 72 }, new RowDefinition { Height = 72 }, new RowDefinition { Height = 72 }, new RowDefinition { Height = 72 } },
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = 72 }, new ColumnDefinition { Width = 72 }, new ColumnDefinition { Width = 72 } },
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = false,
                BackgroundColor = Color.FromRgba(1.0, 1.0, 1.0, 0.7),
                Padding = new Thickness(10, 5, 10, 2)
            };
            customKeyboard.Children.Add(inText, 0, 0);
            for (var j = 0; j < texts.Length; j++)
            {
                customKeyboard.Children.Add(buttons[j], j % 3, 1 + (int)(j / 3));
            }
            Grid.SetColumnSpan(inText, 3);

            var torch = new ImageButton
            {
                Source = "torch.png",
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = 96,
                WidthRequest = 96
            };
            torch.Clicked += delegate
            {
                zxing.ToggleTorch();
            };

            var keyboard = new ImageButton
            {
                Source = "numeric.png",
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = 96,
                WidthRequest = 96
            };
            keyboard.Clicked += delegate
            {
                if (customKeyboard.IsVisible) customKeyboard.IsVisible = false;
                else customKeyboard.IsVisible = true;
            };

            var abort = new Button
            {
                Text = Language.GetText("Cancel"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HeightRequest = 300
            };
            abort.Clicked += delegate
            {
                zxing.RaiseScanResult(null);
            };

            var customOverlayTop = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                Opacity = 0.7,
                Children =
                {
                    new Label
                    {
                        HeightRequest = 20,
                    },
                    new Label
                    {
                        Text = Language.GetText("AppTitle"),
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        TextColor = Color.White
                    },
                    new Label
                    {
                        Text = "Scan QR code...",
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        TextColor = Color.White
                    }
                }
            };

            var customOverlayCenter = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Opacity = 0.7,
                Children =
                {
                    new Image
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.Center,
                        Source = "scanl1.png",
                        Aspect = Aspect.AspectFit
                    },
                    new Image
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Source = "scanl2.png",
                        Aspect = Aspect.AspectFill
                    },
                    new Xamarin.Forms.Shapes.Rectangle
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        WidthRequest = 170,
                        HeightRequest = 170,
                        Stroke = Brush.Black,
                        StrokeThickness = 1,
                        RadiusX = 3,
                        RadiusY = 3
                    },
                    torch,
                    keyboard,
                    customKeyboard
                }
            };

            var customOverlayBottom = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                Opacity = 0.7,
                Padding = new Thickness(0, 10, 0, 10),
            };

            customOverlayBottom.Children.Add(abort);
            //if(zxing.HasTorch) customOverlayBottom.Children.Add(torch); BUG on library, this returns always FALSE
            //customOverlayBottom.Children.Add(torch);

            var customOverlay = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition {Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition {Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition {Height = new GridLength(1, GridUnitType.Star) }
                },
                Children =
                {
                    customOverlayTop,
                    customOverlayCenter,
                    customOverlayBottom
                }
            };

            Grid.SetRow(customOverlayTop, 0);
            Grid.SetRow(customOverlayCenter, 1);
            Grid.SetRow(customOverlayBottom, 2);


            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(zxing);
            grid.Children.Add(customOverlay);

            // The root page of your application
            Content = grid;
        }

        private void Zxing_OnScanResult(ZXing.Result result2)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Stop analysis until we navigate away so we don't keep reading barcodes
                zxing.IsAnalyzing = false;

                result = result2;

                App.Instance.CloseScan();
                //Navigation.PopModalAsync();
            });
        }

        private void SetScanOptions()
        {
            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
            //options.TryHarder = true;
            //options.TryInverted = true;
            //options.AutoRotate = true;
            options.PossibleFormats = new List<ZXing.BarcodeFormat>
            {
                ZXing.BarcodeFormat.QR_CODE
            };
            if (Parameters.Options.AcceptBarcode_Code)
            {
                options.PossibleFormats.Append(ZXing.BarcodeFormat.CODE_39);
                options.PossibleFormats.Append(ZXing.BarcodeFormat.CODE_93);
                options.PossibleFormats.Append(ZXing.BarcodeFormat.CODE_128);
                options.PossibleFormats.Append(ZXing.BarcodeFormat.CODABAR);
            }
            if (Parameters.Options.AcceptBarcode_Ean)
            {
                options.PossibleFormats.Append(ZXing.BarcodeFormat.EAN_8);
                options.PossibleFormats.Append(ZXing.BarcodeFormat.EAN_13);
            }
            if (Parameters.Options.AcceptBarcode_Upc)
            {
                options.PossibleFormats.Append(ZXing.BarcodeFormat.UPC_A);
                options.PossibleFormats.Append(ZXing.BarcodeFormat.UPC_E);
                options.PossibleFormats.Append(ZXing.BarcodeFormat.UPC_EAN_EXTENSION);
            }

            // solve camera resolution bug (up to ZXing 3.1.0 beta2)
            options.CameraResolutionSelector = new CameraResolutionSelectorDelegate((List<CameraResolution> availableResolutions) =>
            {
                CameraResolution result = null;

                double aspectTolerance = 0.1;
                var displayOrientationHeight = DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait ? DeviceDisplay.MainDisplayInfo.Height : DeviceDisplay.MainDisplayInfo.Width;
                var displayOrientationWidth = DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait ? DeviceDisplay.MainDisplayInfo.Width : DeviceDisplay.MainDisplayInfo.Height;

                var targetRatio = displayOrientationHeight / displayOrientationWidth;
                var targetHeight = displayOrientationHeight;
                var minDiff = double.MaxValue;

                foreach (var r in availableResolutions.Where(r => Math.Abs(((double)r.Width / r.Height) - targetRatio) < aspectTolerance))
                {
                    if (Math.Abs(r.Height - targetHeight) < minDiff)
                        minDiff = Math.Abs(r.Height - targetHeight);
                    result = r;
                }
                return result;
            });

            zxing.Options = options;


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            zxing.IsScanning = true;

        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;

            base.OnDisappearing();
        }
    }
}
