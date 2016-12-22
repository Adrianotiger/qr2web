using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace QR2Web
{
	public class App : Application
	{
		private Button ScanButton;
		private Button HistoryButton;
		private Button MoreButton;
		private WebView WebPageWebView;
		private Scanner QRScanner;

		private List<KeyValuePair<DateTime, string>> History = new List<KeyValuePair<DateTime, string>>(16);

		public static App Instance { get; set; } = null;	// Used to access App from the different OS codes
		public static int AppVersion { get; } = 12;         // Version of this app for the different OS codes

		/// <summary>
		/// Constructor. Will initialize the view (for Xamarin), load the parameters and initialize the QR-scanner.
		/// </summary>
		public App ()
		{
			Instance = this;
			QRScanner = new Scanner();						// initialize scanner class (ZXing scanner)

			Parameters.LoadHistory(ref History);			// load scan results history
			Parameters.LoadOptions();						// load app options

			if (!Parameters.Options.SaveHistory) History.Clear();	// clear history if no history should be used
			Language.SetLanguage(Parameters.Options.LanguageIndex); // set language for the app from options

			// create top-bar buttons
			ScanButton = new Button
			{
				Image = new FileImageSource
				{					
					File = "scanb.png",
				},
			};
			ScanButton.Clicked += (sender, e) =>
			{
				StartScan();
			};

			HistoryButton = new Button
			{
				Image = new FileImageSource
				{
					File = "scanh.png",
				},
			};
			HistoryButton.Clicked += HistoryScan_Clicked;
			MoreButton = new Button
			{
				Image = new FileImageSource
				{
					File = "scanm.png",
				},
			};
			MoreButton.Clicked += MoreScan_Clicked;

			// create webView for the page
			WebPageWebView = new WebView
			{
				Source = new UrlWebViewSource
				{
					Url = Parameters.Options.HomePage,
				},
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			WebPageWebView.Navigating += (sender, /*WebNavigatingEventArgs*/ e) =>
			{
					// catch the pressed link. If link is a valid app protocol (qr2web, barcodereader, ...) start QR scanner
				if (Scanner.IsAppURL(e.Url))
				{
					e.Cancel = true;
					StartScanFromWeb(e.Url.ToString());
				}
			};

			// Don't show button if there is no QR-history 
			HistoryButton.IsVisible = Parameters.Options.SaveHistory;

			// The root page of your application
			MainPage = new ContentPage
			{
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Fill,
					HorizontalOptions = LayoutOptions.Fill,
					Children = {
						new StackLayout
						{
							VerticalOptions = LayoutOptions.Fill,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							HeightRequest = 50,
							Orientation = StackOrientation.Horizontal,
							Children = {
								new Label
								{
									Text = Language.GetText("AppTitle"),
									HorizontalOptions = LayoutOptions.StartAndExpand,
									VerticalOptions = LayoutOptions.Fill,
									VerticalTextAlignment = TextAlignment.Center,
									TextColor = Color.Yellow
								},
								ScanButton,
								HistoryButton,
								MoreButton
							}

						},
						WebPageWebView
					}
				},
				Padding = new Thickness(0),
				BackgroundColor = Color.Black,
			};
		}

		/// <summary>
		/// More button was pressed. A list of available action will be showed. Once pressed the attached action will be executed.
		/// </summary>
		/// <param name="sender">Who called this function</param>
		/// <param name="e">Event-Arguments for the Click-Event</param>
		/// <remarks>
		/// </remarks>
		private async void MoreScan_Clicked(object sender, EventArgs e)
		{
			string[] buttons = { Language.GetText("RefreshPage"), Language.GetText("HomePage"), Language.GetText("Settings"), Language.GetText("Help"), Language.GetText("About") };
			
			string result = await MainPage.DisplayActionSheet(Language.GetText("Options"), Language.GetText("Cancel"), null, buttons);

			if (buttons.Contains(result))
			{
				if(result.CompareTo(Language.GetText("Settings")) == 0)
				{
					await App.Current.MainPage.Navigation.PushModalAsync(new OptionsPage());
				}
				else if (result.CompareTo(Language.GetText("RefreshPage")) == 0)
				{
					WebPageWebView.Source = (WebPageWebView.Source as UrlWebViewSource).Url;
				}
				else if (result.CompareTo(Language.GetText("HomePage")) == 0)
				{
					WebPageWebView.Source = Parameters.Options.HomePage;
				}
				else if (result.CompareTo(Language.GetText("Help")) == 0)
				{
					WebPageWebView.Source = "https://adrianotiger.github.io/qr2web/help.html";
				}
				else if (result.CompareTo(Language.GetText("About")) == 0)
				{
					WebPageWebView.Source = "https://adrianotiger.github.io/qr2web/info.html?version=" + AppVersion.ToString() + "&os=" + Device.OS.ToString();
				}

			}
		}

		/// <summary>
		/// History button was pressed. The last parsed QR-codes will be listed.
		/// </summary>
		/// <param name="sender">Who called this function</param>
		/// <param name="e">Event-Arguments for the Click-Event</param>
		/// <remarks>
		/// Helpfull if you was offline and hadn't access to the webpage.
		/// Time are displayed. If the history is older than 1 day, the day will be displayed.
		/// </remarks>
		private async void HistoryScan_Clicked(object sender, EventArgs e)
		{
			string[] buttons = new string[History.Count];
			
			for (int i=0;i<History.Count;i++)
			{
				if(History[i].Key.Day == DateTime.Now.Day)
					buttons[i] = History[i].Value + " (" + History[i].Key.ToString("H:mm:ss") + ")";
				else if (History[i].Key.Day == DateTime.Now.AddDays(-1).Day)
					buttons[i] = History[i].Value + " (" + History[i].Key.ToString("d.MMM H:mm") + ")";
				else
					buttons[i] = History[i].Value + " (" + History[i].Key.ToString("d.MMMM.yyyy") + ")";
			}

			string result = await MainPage.DisplayActionSheet(Language.GetText("History"), Language.GetText("Cancel"), null, buttons);

			if(result.IndexOf("(") > 0)
			{
				OpenJSFunction(result.Substring(0, result.IndexOf("(") - 1));
			}
		}

		/// <summary>
		/// Start the QR-scan page (ZXing library). Will add the scan result to history and execute the Javascript-function on the webpage.
		/// </summary>
		public async void StartScan()
		{
			ZXing.Result result = await QRScanner.StartScan();

			if (result != null)
			{
				AddHistory(result.Text);
				OpenJSFunction(result.Text);
			}

			Parameters.TemporaryOptions.ResetOptions();
		}

		/// <summary>
		/// Execute the callback function in the webpage, passing the parsed QR code.
		/// </summary>
		/// <param name="scanCode">Code or text parsed from barcode</param>
		public void OpenJSFunction(string scanCode)
		{
			string jsString = QRScanner.GenerateJavascriptString(scanCode);

			if(jsString.Length > 10)
			{

			}
			try
			{
				WebPageWebView.Eval(jsString);				
			}
			catch (Exception)
			{
				// BarcodeScanner non available

			}
		}

		/// <summary>
		/// Add the QR-code to history. So it can be used later.
		/// </summary>
		/// <param name="value">The QR code or the parsed value (can be text)</param>
		public void AddHistory(string value)
		{
			for(int i=0;i<History.Count;i++)
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

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
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
			if(url.StartsWith("mochabarcode:"))
			{
				urlWithoutProtocol = url.Substring("mochabarcode:".Length).TrimStart( new char [ '/' ]);
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
					Parameters.SaveParams();
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
			else if(url.StartsWith("p2spro"))
			{
				urlWithoutProtocol = url.Substring("p2spro:".Length).TrimStart(new char['/']);

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
					Parameters.SaveParams();
				}
				else if (urlWithoutProtocol.StartsWith("scan?", StringComparison.CurrentCultureIgnoreCase) ||
					urlWithoutProtocol.StartsWith("scan/?", StringComparison.CurrentCultureIgnoreCase))

				{
					if(urlWithoutProtocol.Contains("callback="))
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
			else if(url.StartsWith("qr2web"))
			{
				urlWithoutProtocol = url.Substring("qr2web:".Length).TrimStart(new char['/']);

				Instance.StartScan();
			}
			// no protocol found, start the normal scan and return the result in the standard function
			else if(Instance != null)
			{
				Instance.StartScan();
			}
		}
		
	}
}
