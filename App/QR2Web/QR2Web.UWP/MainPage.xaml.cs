using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace QR2Web.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			this.InitializeComponent();
			
			LoadApplication(new QR2Web.App());

			InitOSSettings();
			InitExternalLibraries();
			ShowFullScreen();
			LockPortrait(Parameters.Options.LockPortrait);
		}

		public void ShowFullScreen()
		{
			// no full screen on desktop PC
			if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Phone)
			{
				ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
				ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
			}
			else
			{
				ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
			}
		}

		public void LockPortrait(bool tryToLock)
		{
			if (tryToLock)
			{
				DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
			}
		}

		public void InitExternalLibraries()
		{
			ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer.Init();
		}

		public void InitOSSettings()
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
		}
	}
}
