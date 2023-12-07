namespace qr2web
{
    public partial class AppShell : Shell
    {
        private static ScanPage? scanPage = null;

        public static Location? MyLocation { get; private set; } = null;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("OptionsPage", typeof(OptionsPage));
            Routing.RegisterRoute("ScanPage", typeof(ScanPage));

            Task.Run(() =>
                scanPage = new ScanPage());
        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            OpenScanPage();
            //Shell.Current.Navigation.PushAsync(scanPage);
        }

        private void Options_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new OptionsPage());
        }

        private void Refresh_Clicked(object sender, EventArgs e)
        {
            if(Current.CurrentPage is MainPage page)
            {
                page.RefreshPage();
                Shell.Current.FlyoutIsPresented = false;
            }
        }

        private void Home_Clicked(object sender, EventArgs e)
        {
            if (Current.CurrentPage is MainPage page)
            {
                page.GoTo(Options.HomePage);
                Shell.Current.FlyoutIsPresented = false;
            }
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            if (Current.CurrentPage is MainPage page)
            {
                page.GoTo("https://adrianotiger.github.io/qr2web/help.html");
                Shell.Current.FlyoutIsPresented = false;
            }
        }

        private void About_Clicked(object sender, EventArgs e)
        {
            if (Current.CurrentPage is MainPage page)
            {
                int appVer = AppInfo.Current.Version.Major * 10 + AppInfo.Current.Version.Minor;
                page.GoTo("https://adrianotiger.github.io/qr2web/info.html?version=" + appVer);
                Shell.Current.FlyoutIsPresented = false;
            }
        }

        private void Shell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            string currentUrl = e.Current.Location.OriginalString;
            var prevUrl = e.Previous?.Location;
            if(prevUrl != null)
            {
                if (prevUrl.OriginalString.Contains("ScanPage"))
                {
                    Task.Run(async () =>
                    {
                        scanPage = null;
                        await Task.Delay(500);
                        scanPage = new ScanPage();
                    });
                }
            }
            if (Options.ShowScanbutton)
                scanIcon.IsVisible = currentUrl.EndsWith("MainPage");
            else
                scanIcon.IsVisible = false;
         }

        public static async void OpenScanPage()
        {
            if (scanPage != null)
            {
                await Shell.Current.Navigation.PushAsync(scanPage);
                if (Options.ForwardLocation)
                {
                    try
                    {
                        MyLocation = await Geolocation.GetLastKnownLocationAsync();
                        if (MyLocation == null)
                        {
                            await Geolocation.GetLocationAsync(new GeolocationRequest { DesiredAccuracy = GeolocationAccuracy.Medium, RequestFullAccuracy = false, Timeout = TimeSpan.FromSeconds(3) });
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
