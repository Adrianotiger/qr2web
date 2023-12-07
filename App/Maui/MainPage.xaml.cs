
using CommunityToolkit.Maui.Alerts;
using qr2web.Resources.Strings;

namespace qr2web
{
    public partial class MainPage : ContentPage, IQueryAttributable
    {
        public static WebView? MyWebView { get; private set; } = null;
        private readonly List<string> InterceptSchemes = [];

        public MainPage()
        {
            InitializeComponent();

            InterceptSchemes.Add("qr2web:/torch/");
            InterceptSchemes.Add("qr2web:/parameters/");
            InterceptSchemes.Add("qr2web:/scan");

            Options.InitOptions();

            webView.Source = Options.HomePage;
            
            MyWebView = webView;

            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                ExecuteScheme(args[1]);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if(webView.CanGoBack)
            {
                webView.GoBack();
                return true;
            }
            return base.OnBackButtonPressed();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if(query.TryGetValue("code", out object? code) && code is string c)
            {
                InsertBarcode(c);
            }
        }

        public void InsertBarcode(string code)
        {
            var jscall = "window.setTimeout(function() {";
            jscall += "try {";
            jscall += "   if (\"function\" === typeof onQR2WebCodeScan){";
            jscall += "       onQR2WebCodeScan('" + code + "');}";
            jscall += "   else if (\"function\" === typeof onscan){";
            jscall += "       onscan('" + code + "');}";
            jscall += "}catch(e){alert(\"insertScannedCode or onscan not found\")}}, 100);";
            InjectJS(jscall);

            if(Options.ForwardLocation)
            {
                double latitude = (AppShell.MyLocation?.Latitude != null) ? AppShell.MyLocation.Latitude : 47.5;
                double longitude = (AppShell.MyLocation?.Longitude != null) ? AppShell.MyLocation.Longitude : 9.5;
                jscall = "window.setTimeout(function() {";
                jscall += "try {";
                jscall += "   if (\"function\" === typeof onQR2WebLocation){";
                jscall += "       onQR2WebLocation('{\"latitude\":\"" + latitude + "\", \"longitude\":\"" + longitude + "\"}');}";
                jscall += "}catch(e){alert(\"onQR2WebLocation error\")}}, 200);";
                InjectJS(jscall);
            }
        }

        public void RefreshPage()
        {
            webView.Reload();
        }

        public void GoTo(string url = "")
        {
            if (url == "") url = Options.HomePage;
            webView.Source = url;
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Url.StartsWith("http"))// http or https
            {
                InjectJS("window.QR2WEB=" + AppInfo.Current.BuildString);
            }
            progressBar.IsVisible = false;
        }

        public void InjectJS(string js)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await webView.EvaluateJavaScriptAsync(js);
                }
                catch (Exception e)
                {
                    // Unable to inject js source
                    Console.WriteLine(e);
                }
            });
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            string[] interceptScans = ["qr2web:", "p2spro:", "readbarcode:", "mochabarcode:"];
            if(interceptScans.Any(s => e.Url.StartsWith(s, StringComparison.CurrentCultureIgnoreCase)))
            {
                e.Cancel = true;
                if (e.Url.StartsWith(interceptScans[0]))
                {
                    if (ExecuteScheme(e.Url)) return;
                }
                AppShell.OpenScanPage();
                return;
            }
            else if(!e.Url.StartsWith("http"))
            {
                OpenExternalLink(e.Url);
                e.Cancel = true;
            }
            else
            {
                progressBar.Progress = 0.0;
                progressBar.IsVisible = true;
                progressBar.ProgressTo(0.8, 5000, Easing.Linear);
            }
        }

        private bool ExecuteScheme(string url)
        {
            int schemeIndex = -1;
            bool schemeFound = false;
            string urlParam = "";
            InterceptSchemes.ForEach(s =>
            {
                if (schemeFound) return;

                schemeIndex++;
                if (url.StartsWith(s))
                {
                    schemeFound = true;
                    urlParam = url[s.Length..].Trim('/');
                    return;
                }
            });

            if (schemeFound)
            {
                switch (schemeIndex)
                {
                    case 0:
                        SetTorch(urlParam);
                        break;
                    case 1:
                        SaveNewParams(urlParam);
                        break;
                    case 2:
                        AppShell.OpenScanPage();
                        break;
                }
            }
            return schemeFound;
        }

        private static void SetTorch(string param)
        {
            try
            {
                if (param == "on")
                {
                    Flashlight.TurnOnAsync();
                }
                else if (param == "off")
                {
                    Flashlight.TurnOffAsync();
                }
                else if (int.TryParse(param, out int onTime))
                {
                    Task.Run(async () =>
                    {
                        await Flashlight.TurnOnAsync();
                        await Task.Delay(onTime);
                        await Flashlight.TurnOffAsync();
                    });
                }
            }
            catch { }
        }

        private async void SaveNewParams(string param, bool update = false)
        {
            if (!Options.CheckStringParam(param, out string errors, out Dictionary<string, string> keyValuePairs))
            {
                var toast = Toast.Make(errors, CommunityToolkit.Maui.Core.ToastDuration.Long);
                await toast.Show();
                return;
            }
            else if (errors.Length > 2)
            {
                var toast = Toast.Make(errors, CommunityToolkit.Maui.Core.ToastDuration.Short);
                await toast.Show();
            }

            if(keyValuePairs.Count == 0)
            {
                var toast = Toast.Make(AppResources.ScanParamNoValid, CommunityToolkit.Maui.Core.ToastDuration.Short);
                await toast.Show();
                return;
            }

            string message = AppResources.ScanParamNewFound;
            if (keyValuePairs.Count == 1) message = AppResources.ScanParamNewFound1 + " (" + keyValuePairs.First().Key + ")";
            message += ".\n" + AppResources.ScanParamNewSave;

            if(!update && !await DisplayAlert(AppResources.ScanParamAppSettings, message, AppResources.Yes, AppResources.No))
            {
                return;
            }

            if(Options.UpdateNewParams(param, out _))
            {
                var toast = Toast.Make(AppResources.ScanParamSaved, CommunityToolkit.Maui.Core.ToastDuration.Short);
                await toast.Show();
            }
            else
            {

            }
        }

        static public void OpenExternalLink(string url)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);

                string extUrl = url;
                if (extUrl.Length > 32) extUrl = string.Concat(extUrl.AsSpan(0, 30), "...");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (Application.Current != null && Application.Current.MainPage != null)
                    {
                        if (url.StartsWith("http"))
                        {
                            if (!Options.ExternalLinks)
                            {
                                var toast = Toast.Make(AppResources.ExternalLinkNotAllowed, CommunityToolkit.Maui.Core.ToastDuration.Short);
                                await toast.Show();
                            }
                            else if (await Application.Current.MainPage.DisplayAlert(AppResources.ExternalLink, AppResources.ExternalLinkAppWantOpen + ":\n" + extUrl + "\n" + AppResources.ExternaLinkAppProceed, AppResources.Yes, AppResources.No))
                            {
                                await Launcher.Default.OpenAsync(url);
                            }
                        }
                        else
                        {
                            await Launcher.Default.OpenAsync(url);
                        }
                    }
                });
            });
        }
    }

}
