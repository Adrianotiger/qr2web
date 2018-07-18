using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

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
			torch.Clicked += delegate {
				zxing.ToggleTorch();
			};

			var abort = new Button
			{
				Text = Language.GetText("Cancel"),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				TextColor = Color.White
			};
			abort.Clicked += delegate {
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

				Navigation.PopModalAsync();
			});
		}

		private void SetScanOptions()
		{
			var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
			//options.TryHarder = true;
			//options.TryInverted = true;
			//options.AutoRotate = true;
			options.PossibleFormats = new List<ZXing.BarcodeFormat>();
			options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
			if (Parameters.Options.AcceptBarcode_Code)
			{
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_39);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_93);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_128);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODABAR);
			}
			if (Parameters.Options.AcceptBarcode_Ean)
			{
				options.PossibleFormats.Add(ZXing.BarcodeFormat.EAN_8);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.EAN_13);
			}
			if (Parameters.Options.AcceptBarcode_Upc)
			{
				options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_A);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_E);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_EAN_EXTENSION);
			}

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
