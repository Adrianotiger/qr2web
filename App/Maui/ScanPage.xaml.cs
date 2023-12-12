using qr2web.Resources.Strings;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace qr2web;

public partial class ScanPage : ContentPage
{
    private CameraBarcodeReaderView? cameraBarcodeReaderView = null;

    public ScanPage()
	{
        InitializeComponent();

        Task.Run(async() =>
        {
            // Flashlight.IsSupportedAsync hangs on Windows, if called when creating the page.
            // Wait 200ms to check if the flashlight is supported.
            await Task.Delay(200);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool HasTorch = await Flashlight.IsSupportedAsync();
                flashButton.IsVisible = HasTorch;
            });
        });

        parsedText.Loaded += ParsedText_Loaded;
    }

    private void ParsedText_Loaded(object? sender, EventArgs e)
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(500);

            BarcodeFormat format = BarcodeFormat.QrCode;
            if (Options.Barcodes[Options.BarcodeType.Code])
            {
                format |= BarcodeFormat.Code39 | BarcodeFormat.Code93 | BarcodeFormat.Code128 | BarcodeFormat.Codabar;
            }
            if (Options.Barcodes[Options.BarcodeType.Ean])
            {
                format |= BarcodeFormat.Ean8 | BarcodeFormat.Ean13;
            }
            if (Options.Barcodes[Options.BarcodeType.Upc])
            {
                format |= BarcodeFormat.UpcA | BarcodeFormat.UpcE | BarcodeFormat.UpcEanExtension;
            }
            if (Options.Barcodes[Options.BarcodeType.Other1d])
            {
                format |= BarcodeFormat.Itf | BarcodeFormat.Rss14 | BarcodeFormat.RssExpanded;
            }

            cameraBarcodeReaderView = new CameraBarcodeReaderView
            {
                VerticalOptions = LayoutOptions.Fill
            };

            cameraBarcodeReaderView.BarcodesDetected += BarcodesDetected;

            cameraBarcodeReaderView.Options = new BarcodeReaderOptions
            {
                Formats = format,
                AutoRotate = true,
                Multiple = false
            };

            cameraBarcodeReaderView.Loaded += (s, e) =>
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (cameraBarcodeReaderView.Width > cameraBarcodeReaderView.Height)
                    {
                        cameraBarcodeReaderView.VerticalOptions = LayoutOptions.Start;
                        mainGrid.BackgroundColor = Colors.LightGray;
                    }

                    parsedText.Text = AppResources.ScanPageScanning;
                });
            };

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                parsedText.Text = AppResources.ScanPageStartCamera;

                historyOverlay.ItemsSource = Options.History;
                historyOverlay.IsVisible = Options.UseHistory;

                entryText.Keyboard = Keyboard.Create(KeyboardFlags.None);
                if (Options.Keyboard != Options.KeyboardType.Overlay)
                {
                    foreach (var item in keyboardOverlay.Children)
                    {
                        if (item is Button button)
                        {
                            button.IsVisible = false;
                        }
                    }
                    keyboardOverlay.SetRow(entryText, 2);
                    switch(Options.Keyboard)
                    {
                        case Options.KeyboardType.Numeric:
                            entryText.Keyboard = Keyboard.Numeric;
                            break;
                        case Options.KeyboardType.Plain:
                            entryText.Keyboard = Keyboard.Plain;
                            break;
                    }
                    entryText.Completed += (s, e) =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Options.AddHistoryScan(new HistoryData { Type = Options.BarcodeType.Keyboard, Date = DateTime.Now, Scan = entryText.Text });
                            App.InsertCode(entryText.Text);
                        });
                    };

                }
                else
                {
                    entryText.IsReadOnly = true;
                }

                mainGrid.Insert(0, cameraBarcodeReaderView);
                historyOverlay.IsVisible = false;
            });
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        historyButton.IsVisible = Options.UseHistory;
        keyboardButton.IsVisible = Options.Keyboard != Options.KeyboardType.Disabled;
    }

    protected override async void OnDisappearing()
    {
        if (cameraBarcodeReaderView != null && cameraBarcodeReaderView.IsTorchOn)
        {
            cameraBarcodeReaderView.IsTorchOn = false;
            await Task.Delay(100);
        }

        base.OnDisappearing();
    }

    protected void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        BarcodeResult? result = null;
        if (cameraBarcodeReaderView == null) return;

        foreach (var barcode in e.Results)
        {
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");
            result = barcode;
        }

        if (result != null && cameraBarcodeReaderView.IsDetecting)
        {
            cameraBarcodeReaderView.IsDetecting = false;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Options.AddHistoryScan(new HistoryData { Type = Options.BarcodeType.QR, Date = DateTime.Now, Scan = result.Value });
                App.InsertCode(result.Value);
            });
        }
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.Navigation.PopToRootAsync();
    }

    private void TorchButton_Clicked(object sender, EventArgs e)
    {
        if (cameraBarcodeReaderView == null) return;

        cameraBarcodeReaderView.IsTorchOn = !cameraBarcodeReaderView.IsTorchOn;
    }

    private void HistoryButton_Clicked(object sender, EventArgs e)
    {
        if(!historyOverlay.IsVisible)
        {
            historyOverlay.IsVisible = true;
            keyboardOverlay.IsVisible = false;
            if(cameraBarcodeReaderView != null) cameraBarcodeReaderView.IsTorchOn = false;

        }
        else
        {
            historyOverlay.IsVisible = false;
        }
    }

    private async void KeyButton_Clicked(object sender, EventArgs e)
    {
        if(keyboardOverlay.IsVisible)
        {
            keyboardOverlay.IsVisible = false;
        }
        else
        {
            keyboardOverlay.IsVisible = true;
            historyOverlay.IsVisible = false;
            if (cameraBarcodeReaderView != null) cameraBarcodeReaderView.IsTorchOn = false;
            entryText.Text = string.Empty;

            if(Options.Keyboard == Options.KeyboardType.Numeric)
            {
                await entryText.ShowSoftInputAsync(System.Threading.CancellationToken.None);
            }
            else if (Options.Keyboard == Options.KeyboardType.Plain)
            {
                await entryText.ShowSoftInputAsync(System.Threading.CancellationToken.None);
            }
        }
    }

    private void KeyPressed(object sender, EventArgs e)
    {
        entryText.Text += (sender as Button)?.Text;
    }

    private void KeyPressed_Back(object sender, EventArgs e)
    {
        string? text = entryText.Text;
        if (text == null || text.Length < 1) text = "0";

        entryText.Text = text[..^1];
    }

    private void KeyPressed_Enter(object sender, EventArgs e)
    {
        if(entryText.Text.Length > 0)
        {
            Options.AddHistoryScan(new HistoryData { Type = Options.BarcodeType.Keyboard, Date = DateTime.Now, Scan = entryText.Text });
            App.InsertCode(entryText.Text);
        }
    }

    private void HistoryOverlay_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var code = (e.SelectedItem as HistoryData)?.Scan;
        if (code != null)
        {
            //Options.AddHistoryScan(new HistoryData { Type = Options.BarcodeType.History, Date = DateTime.Now, Scan = code });
            App.InsertCode(code);
        }
    }
}