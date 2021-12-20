using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QR2Web
{
    public class App : Application
    {
        private Scanner QRScanner = null;
        private QRMainPage QRPage = null;

        public static List<KeyValuePair<DateTime, string>> History = new List<KeyValuePair<DateTime, string>>(16);

        public static App Instance { get; set; } = null;    // Used to access App from the different OS codes
        public static int AppVersion { get; } = 21;         // Version of this app for the different OS codes

        public static bool HasBackButton { get; private set; }
        public static OSInterface IOS { get; private set; }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
        }

        /// <summary>
        /// Constructor. Will initialize the view (for Xamarin), load the parameters and initialize the QR-scanner.
        /// </summary>
        public App(OSInterface osi, bool withBackButton = false)
        {
            Instance = this;
            HasBackButton = withBackButton;
            IOS = osi;

            MainPage = new LauncherPage();
        }

        public void LoadMainPage(bool baseInit = true)
        {
            if (baseInit)
            {
                Language.SetLanguage(Parameters.Options.LanguageIndex); // set language for the app from options	

                QRPage = new QRMainPage();
            }
            else
            {
                QRScanner = new Scanner();                      // initialize scanner class (ZXing scanner)

                Parameters.LoadHistory(ref History);            // load scan results history

                if (Parameters.Options.UseLocation)
                    QRLocation.InitLocation();

                if (!Parameters.Options.SaveHistory) History.Clear();   // clear history if no history should be used

                QRPage.Initialize();
            }
            
        }

        public bool IsMainPageReady()
        {
            return QRPage.IsLoaded();
        }

        public void StartMainPage()
        {
            NavigateTo(QRPage);
        }

        public void NavigateTo(Page p)
        {
            (MainPage as LauncherPage).AddPage(p);
        }

        public bool GoBack()
        {
            if (QRPage.Back()) return true;
            return false;
        }

        public async void CloseScan()
        {
            var n = (MainPage as LauncherPage).Navigation;
            if (n.NavigationStack[n.NavigationStack.Count - 1].GetType() == typeof(CustomScanPage))
            {
                await n.PopAsync();
            }
        }

        /// <summary>
        /// Start the QR-scan page (ZXing library). Will add the scan result to history and execute the Javascript-function on the webpage.
        /// </summary>
        public void StartScan()
        {
            //////////////// TEST
            CustomScanPage customPage = new CustomScanPage();

            customPage.Disappearing += (s, e) =>
            {
                ZXing.Result result = customPage.result;

                if (result != null)
                {
                    AddHistory(result.Text);
                    OpenJSFunctionQRCode(result.Text);
                    if (Parameters.Options.UseLocation)
                    {
                        OpenJSFunctionLocation();
                    }
                }

                Parameters.TemporaryOptions.ResetOptions();
            };

            if (Parameters.Options.UseLocation)
            {
                QRLocation.InitLocation();
            }

            NavigateTo(customPage);
            //await App.Current.MainPage.Navigation.PushModalAsync(customPage);
        }

        /// <summary>
        /// Execute the callback function in the webpage, passing the parsed QR code.
        /// </summary>
        /// <param name="scanCode">Code or text parsed from barcode</param>
        public void OpenJSFunctionQRCode(string scanCode)
        {
            string jsString = QRScanner.GenerateJavascriptString(scanCode);

            QRPage.InjectJS(jsString);
        }

        /// <summary>
        /// Execute the callback function in the webpage, passing the current location.
        /// </summary>
        /// <param name="scanCode">Code or text parsed from barcode</param>
        public void OpenJSFunctionLocation()
        {
            string jsString = QRLocation.GenerateJavascriptString();
            QRPage.InjectJS(jsString);
        }

        /// <summary>
        /// Add the QR-code to history. So it can be used later.
        /// </summary>
        /// <param name="value">The QR code or the parsed value (can be text)</param>
        public void AddHistory(string value)
        {
            for (int i = 0; i < History.Count; i++)
            {
                if (History[i].Value == value)
                {
                    History.RemoveAt(i);
                }
            }
            if (Parameters.Options.SaveHistory)
            {
                History.Insert(
                        0,
                        new KeyValuePair<DateTime, string>
                        (
                            DateTime.Now,
                            value
                        )
                    );

                Parameters.SaveHistory(ref History);
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }



        /// <summary>
        /// Scan request was made from Web (through Link or AppLink).
        /// </summary>
        /// <param name="url">Who called this function</param>
        /// <remarks>
        /// The app can be started with a SCAN-call through the webpage: "http://SCAN/". This will work only if the link is opened in this app.<br></br>
        /// You can also open the scanner with "qr2web://scan/". This will open this app even if you are using your default webbrowser.
        /// </remarks>
        static public void StartScanFromWeb(string url)
        {
            string urlWithoutProtocol;
            // Check Mocha protocols
            if (url.StartsWith("mochabarcode:"))
            {
                urlWithoutProtocol = url.Substring("mochabarcode:".Length).TrimStart('/');
                if (urlWithoutProtocol.StartsWith("CONFIG", StringComparison.CurrentCultureIgnoreCase))
                {
                    //mochabarcode://CONFIG=http://mochasoft.com/test1.htm&autolock=1&function=1&field=1&
                    //history = 1 & scan = 1 & back = 1 & forward = 1 & reload = 1 & reloadstart = 1 & shake = 0
                    //& ignorefirst = 0 & ignorelast = 0 & beep = 1
                    string[] sParams = urlWithoutProtocol.Remove(' ').Split('&');
                    foreach (string sParam in sParams)
                    {
                        if (sParam.IndexOf('=') > 0)
                        {
                            string value = (sParam.Split('='))[1].ToLower();
                            switch ((sParam.Split('='))[0].ToLower())
                            {
                                case "CONFIG":
                                    if (value.Length > 5)
                                    {
                                        Parameters.Options.HomePage = value;
                                    }
                                    break;
                                case "history":
                                    if (value == "1")
                                        Parameters.Options.SaveHistory = true;
                                    else
                                        Parameters.Options.SaveHistory = false;
                                    break;
                            }
                        }
                    }
                }
                else if (urlWithoutProtocol.StartsWith("CALLBACK=", StringComparison.CurrentCultureIgnoreCase))
                {
                    Parameters.TemporaryOptions.SetLookup("", urlWithoutProtocol.Substring("CALLBACK=".Length), Parameters.EmulationTypes.MOCHASOFT);
                    Instance.StartScan();
                }
                else
                {
                    Parameters.TemporaryOptions.SetLookup("", "", Parameters.EmulationTypes.MOCHASOFT);
                    Instance.StartScan();
                }

            }
            // check pic 2 shop pro protocol
            else if (url.StartsWith("p2spro"))
            {
                urlWithoutProtocol = url.Substring("p2spro:".Length).TrimStart('/');

                if (urlWithoutProtocol.StartsWith("configure?", StringComparison.CurrentCultureIgnoreCase)
                    || urlWithoutProtocol.StartsWith("configure/?", StringComparison.CurrentCultureIgnoreCase))
                {
                    //p2spro://configure?lookup=LOOKUP_URL&home=HOME_URL&formats=EAN13,EAN8,UPCE,ITF,CODE39,CODE128,CODE93,STD2OF5,CODABAR,QR &gps=True|False
                    //&hidebuttons = True | False
                    //& autorotate = True | False
                    //& highres = True | False
                    //& settings = True | False
                    string[] sParams = urlWithoutProtocol.Substring(
                            (urlWithoutProtocol.StartsWith("configure?", StringComparison.CurrentCultureIgnoreCase) ?
                            "configure?".Length :
                            "configure/?".Length)
                        ).Remove(' ').Split('&');
                    foreach (string sParam in sParams)
                    {
                        if (sParam.IndexOf('=') > 0)
                        {
                            string value = (sParam.Split('='))[1].ToLower();
                            switch ((sParam.Split('='))[0].ToLower())
                            {
                                case "home":
                                    if (value.Length > 5)
                                    {
                                        Parameters.Options.HomePage = value;
                                    }
                                    break;
                                case "autorotate":
                                    if (value.ToLower() == "true")
                                        Parameters.Options.LockPortrait = true;
                                    else
                                        Parameters.Options.LockPortrait = false;
                                    break;
                                case "formats":
                                    Parameters.Options.AcceptBarcode_Code = value.ToLower().Contains("code");
                                    Parameters.Options.AcceptBarcode_Ean = value.ToLower().Contains("ean");
                                    Parameters.Options.AcceptBarcode_Upc = value.ToLower().Contains("upc");
                                    break;
                            }
                        }
                    }
                }
                else if (urlWithoutProtocol.StartsWith("scan?", StringComparison.CurrentCultureIgnoreCase) ||
                    urlWithoutProtocol.StartsWith("scan/?", StringComparison.CurrentCultureIgnoreCase))

                {
                    if (urlWithoutProtocol.Contains("callback="))
                    {
                        string callbackCommand = url.Substring(url.IndexOf("callback=", StringComparison.CurrentCultureIgnoreCase) + "callback=".Length);
                        if (callbackCommand.IndexOf('&') > 0) callbackCommand = callbackCommand.Substring(0, callbackCommand.IndexOf('&'));
                        if (callbackCommand.StartsWith("javascript:", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Parameters.TemporaryOptions.SetLookup(callbackCommand.Substring("javascript:".Length), "", Parameters.EmulationTypes.PIC2SHOP);
                        }
                        else
                        {
                            Parameters.TemporaryOptions.SetLookup("", callbackCommand, Parameters.EmulationTypes.PIC2SHOP);
                        }
                    }
                    Instance.StartScan();
                }
                else
                {
                    Instance.StartScan();
                }
            }
            // check qr to Web protocol
            else if (url.StartsWith("qr2web"))
            {
                urlWithoutProtocol = url.Substring("qr2web:".Length).TrimStart('/');

                if (urlWithoutProtocol.ToLower().StartsWith("parameters"))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        SaveNewParameters(urlWithoutProtocol.Substring("parameters".Length).TrimStart('/'));
                    });
                }
                else if (urlWithoutProtocol.ToLower().StartsWith("torch"))
                {
                    var time = urlWithoutProtocol.ToLower().Split("/");
                    if(time.Length == 2 && time[1] != "off")
                    {
                        new Task(async () =>
                        {
                            try
                            {
                                MainThread.BeginInvokeOnMainThread(async () =>
                                {
                                    await Flashlight.TurnOnAsync();
                                });
                                int timeout = 2000;
                                int.TryParse(time[1], out timeout);
                                if (timeout < 1000) timeout = 1000;
                                else if (timeout > 30000) timeout = 30000;
                                await Task.Delay(timeout);
                                MainThread.BeginInvokeOnMainThread(async () =>
                                {
                                    await Flashlight.TurnOffAsync();
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex);
                            }
                        }).Start();
                    }
                    else
                    {
                        try
                        {
                            Xamarin.Essentials.Flashlight.TurnOffAsync();
                        }
                        catch(Exception ex)
                        {
                            Console.Write(ex);
                        }
                    }
                }
                else
                {
                    Instance.StartScan();
                }
            }
            // no protocol found, start the normal scan and return the result in the standard function
            else if (Instance != null)
            {
                Instance.StartScan();
            }
        }

        public static async void SaveNewParameters(string paramString)
        {

            Dictionary<string, string> dicQueryString =
                        paramString.Split('&')
                             .ToDictionary(c => c.Split('=')[0],
                                           c => Uri.UnescapeDataString(c.Split('=')[1]));

            if (Instance != null)
            {
                string changes = "";
                if (dicQueryString.ContainsKey("webpage")) changes += "webpage='" + dicQueryString["webpage"] + "'\n";
                if (dicQueryString.ContainsKey("portrait")) changes += "lock portrait='" + dicQueryString["portrait"] + "'\n";
                if (dicQueryString.ContainsKey("gps")) changes += "location='" + dicQueryString["gps"] + "'\n";
                if (dicQueryString.ContainsKey("language")) changes += "language='" + dicQueryString["language"] + "'\n";

                if (!await Instance.MainPage.DisplayAlert(Language.GetText("OptionChange_1"), Language.GetText("OptionChange_2") + "\n" + changes + Language.GetText("OptionChange_3"), Language.GetText("Yes"), Language.GetText("No")))
                {
                    return;
                }

            }

            if (dicQueryString.ContainsKey("webpage"))
            {
                Parameters.Options.HomePage = dicQueryString["webpage"];
            }
            if (dicQueryString.ContainsKey("portrait"))
            {
                bool val = true;
                bool.TryParse(dicQueryString["portrait"], out val);
                Parameters.Options.LockPortrait = val;
            }
            if (dicQueryString.ContainsKey("gps"))
            {
                bool val = true;
                bool.TryParse(dicQueryString["gps"], out val);
                Parameters.Options.UseLocation = val;
            }
            if (dicQueryString.ContainsKey("language"))
            {
                Language.SetLanguage(dicQueryString["language"]);
            }

            if (dicQueryString.ContainsKey("webpage"))
            {
                Instance.QRPage.SetSource(Parameters.Options.HomePage);
            }
        }

    }
}
