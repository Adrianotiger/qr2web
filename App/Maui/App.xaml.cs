
namespace qr2web
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Options.InitOptions();

            MainPage = new AppShell();
        }

        public static async void InsertCode(string code)
        {
            ShellNavigationQueryParameters param = [];
            param.Add("code", code);
            await Shell.Current.Navigation.PopAsync(true);
            
            if(Shell.Current.CurrentPage is MainPage page)
            {
                page.InsertBarcode(code);
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            int newWidth = Preferences.Get("window_w", 450);
            int newHeight = (int)Preferences.Get("window_h", 800);
            int newX = Preferences.Get("window_x", (int)Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Width / 2 - 225);
            int newY = Preferences.Get("window_y", (int)Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Height / 2 - 400);

            window.Width = newWidth;
            window.Height = newHeight;
            window.X = newX;
            window.Y = newY;

            window.Destroying += (s, e) =>
            {
                if (s is Window w)
                {
                    if (w.Width != newWidth) Preferences.Set("window_w", (int)w.Width);
                    if (w.Height != newHeight) Preferences.Set("window_h", (int)w.Height);
                    if (w.X != newX) Preferences.Set("window_x", (int)w.X);
                    if (w.Y != newY) Preferences.Set("window_y", (int)w.Y);
                }
            };

            window.Deactivated += (s, e) =>
            {
                if (Shell.Current.CurrentPage is MainPage page)
                {
                    //page.SaveState();
                }
            };

            window.Resumed += (s, e) =>
            {
                if (Shell.Current.CurrentPage is MainPage page)
                {
                    //page.RestoreState();
                }
            };

            return window;
        }

        
    }
}
