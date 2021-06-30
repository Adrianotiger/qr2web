using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;
using Frame = Windows.UI.Xaml.Controls.Frame;

namespace QR2Web.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Windows.UI.Xaml.Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void InitXamarin(IActivatedEventArgs e)
        {
            //// Workaround to show the scanner also on Windows.
            List<Assembly> assembliesToInclude = new List<Assembly>();

            assembliesToInclude.Add(typeof(ZXing.BarcodeReader).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Reader).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.BaseLuminanceSource).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Binarizer).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.BinaryBitmap).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Dimension).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.LuminanceSource).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.MultiFormatReader).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.PlanarYUVLuminanceSource).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Result).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.ResultPoint).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.RGBLuminanceSource).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.SupportClass).GetTypeInfo().Assembly);

            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.Forms.ZXingBarcodeImageView).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.Forms.ZXingScannerPage).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.Forms.ZXingScannerView).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.Forms.ZXingDefaultOverlay).GetTypeInfo().Assembly);

            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.ZXing_Net_Mobile_XamlTypeInfo.XamlMetaDataProvider).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingBarcodeImageViewRenderer).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer).GetTypeInfo().Assembly);

            assembliesToInclude.Add(typeof(ZXing.Mobile.MobileBarcodeScanner).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Mobile.MobileBarcodeScannerBase).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Mobile.MobileBarcodeScanningOptions).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Mobile.ScanPage).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Mobile.MobileBarcodeScannerBase).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Mobile.SoftwareBitmapLuminanceSource).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(ZXing.Mobile.ZXingScannerControl).GetTypeInfo().Assembly);

            Xamarin.Forms.Forms.Init(e, assembliesToInclude);
            //// End workaround, the original init is only 1 line:

            //Xamarin.Forms.Forms.Init(e);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                InitXamarin(e);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args is ProtocolActivatedEventArgs)
            {
                ProtocolActivatedEventArgs pargs = (ProtocolActivatedEventArgs)args;
                bool success = false;

                if (pargs.PreviousExecutionState == ApplicationExecutionState.ClosedByUser ||
                    pargs.PreviousExecutionState == ApplicationExecutionState.NotRunning)
                {
                    Frame rootFrame = Window.Current.Content as Frame;

                    // Do not repeat app initialization when the Window already has content,
                    // just ensure that the window is active
                    if (rootFrame == null)
                    {
                        // Create a Frame to act as the navigation context and navigate to the first page
                        rootFrame = new Frame();

                        rootFrame.NavigationFailed += OnNavigationFailed;

                        InitXamarin(args);

                        // Place the frame in the current Window
                        Window.Current.Content = rootFrame;
                    }
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), args);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }

                try
                {
                    if (Scanner.IsAppURL(pargs.Uri.ToString()))
                    {
                        success = true;
                        QR2Web.App.StartScanFromWeb(pargs.Uri.ToString());
                    }
                }
                catch (Exception)
                {
                    success = false;
                }

                if (!success)
                {
                    QR2Web.App.StartScanFromWeb("");
                }
            }
            base.OnActivated(args);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
