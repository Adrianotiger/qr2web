using System;
using System.Collections.Generic;
using System.Linq;
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

            var torch = new Button
            {
                Text = Language.GetText("Torch"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White
            };
            torch.Clicked += delegate
            {
                zxing.ToggleTorch();
            };

            var abort = new Button
            {
                Text = Language.GetText("Cancel"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White
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
                Opacity = 0.6,
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
                    }
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
            customOverlayBottom.Children.Add(torch);

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
