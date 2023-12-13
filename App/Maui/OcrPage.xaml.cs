using Camera.MAUI;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using TesseractOcrMaui;

namespace qr2web;

public partial class OcrPage : ContentPage
{
    ITesseract Tesseract { get; }
    ImageSource CamImage { get; set; }
    SkiaSharp.SKBitmap SkiaBmp { get; set; }
    SkiaSharp.SKRect SkiaSrc { get; set; }
    SkiaSharp.SKRect SkiaDest { get; set; }
    bool Finished = true;

    public OcrPage(ITesseract tesseract)
	{
		InitializeComponent();

        Tesseract = tesseract;
        cameraView.CamerasLoaded += CameraView_CamerasLoaded;
    }

    private void CameraView_CamerasLoaded(object? sender, EventArgs e)
    {
        Tesseract.EngineConfiguration = (engine) =>
        {
            // Engine uses DefaultSegmentationMode, if no other is passed as method parameter.
            // If ITesseract is injected to page, this is only way of setting PageSegmentationMode.
            // PageSegmentationMode defines how ocr tries to look for text, for example singe character or single word.
            // By default uses PageSegmentationMode.Auto.
            engine.DefaultSegmentationMode = TesseractOcrMaui.Enums.PageSegmentationMode.SingleLine;

            //engine.SetCharacterWhitelist("0123456789");   // These characters ocr is looking for
            //engine.SetCharacterBlacklist("abc");        // These characters ocr is not looking for
                                                        // Now ocr should be only finding characters 'defgh'
        };

        if (cameraView.NumCamerasDetected > 0)
        {
            cameraView.Microphone = null;
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (await cameraView.StartCameraAsync() == CameraResult.Success)
                {
                    skiaView.WidthRequest = 800;
                    skiaView.HeightRequest = 300;
                }
            });
        }
    }

    protected override void OnDisappearing()
    {
        cameraView.TorchEnabled = false;
        base.OnDisappearing();
    }

    private void Flash_Clicked(object sender, EventArgs e)
    {
        cameraView.TorchEnabled = !cameraView.TorchEnabled;
    }

    private async void ocrText_Clicked(object sender, EventArgs e)
    {
        var win = App.Current?.Windows[0];

        (sender as Button).IsEnabled = false;

        await Task.Run(async() =>
        {
            try
            {
                while (this != null)
                {
                    await Task.Delay(1000);
                    while (!Finished) await Task.Delay(100);
                    CamImage = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
                    Stream stream = await ((StreamImageSource)CamImage).Stream(CancellationToken.None);
                    SkiaBmp = SkiaSharp.SKBitmap.Decode(stream);

                    float factor = (float)SkiaBmp.Width / SkiaBmp.Height;
                    SkiaSrc = new SkiaSharp.SKRect((float)SkiaBmp.Width * 1 / 3, (float)SkiaBmp.Height * 3 / 13, (float)SkiaBmp.Width * 2 / 3, (float)SkiaBmp.Height * 5 / 13);
                    SkiaDest = new SkiaSharp.SKRect(0, 0, 800, 300);

                    Finished = false;
                    skiaView.InvalidateSurface();
                }
            }
            catch(Exception)
            {
                (sender as Button).IsEnabled = true;
            }
        });

        
    }

    private void OnPaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        if (SkiaBmp == null) return;

        SKPaint paintB = new SKPaint();
        paintB.Color = SKColors.Black;
        paintB.Style = SKPaintStyle.Fill;
        paintB.TextSize = 40;

        e.Surface.Canvas.DrawBitmap(SkiaBmp, SkiaSrc, SkiaDest);
        //e.Surface.Canvas.Flush();
        //e.Surface.Flush();

        OcrBitmap(e.Surface.Snapshot(new SKRectI(0,0,800,300)));
    }

    private async void OcrBitmap(SKImage image)
    {
        await Task.Run(async () =>
        {            
            SKBitmap bmpDest = SKBitmap.FromImage(image);
            bmpDest.SetImmutable();

            using (var data = image.Encode(SKEncodedImageFormat.Png, 95))
            {
                var pix = Pix.LoadFromMemory(data.ToArray());

                var result = await Tesseract.RecognizeTextAsync(pix);

                SKImage image = SKImage.FromPixels(bmpDest.PeekPixels());
                // encode the image (defaults to PNG)
                SKData encoded = image.Encode();
                // get a stream over the encoded data
                Stream stream = encoded.AsStream();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ocrData.Text = "C: " + result.Confidence + result.Message;
                    ocrText.Text = result.RecognisedText;

                    xxxImage.Source = ImageSource.FromStream(() => stream);

                    Finished = true;
                });
            }
        });
    }
}