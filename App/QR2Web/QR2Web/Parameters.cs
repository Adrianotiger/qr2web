using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace QR2Web
{
	/// <summary>
	/// Parameters used to show the page and functionality of the app.
	/// </summary>
    static class Parameters
    {
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		/// <summary>
		/// App can try to emulate some other protocols.
		/// </summary>
		static public class EmulationTypes
		{
			public const int UNKNOWN = -1;
			public const int NORMAL = 0;
			public const int MOCHASOFT = 1;
			public const int PIC2SHOP= 2;
		}

		/// <summary>
		/// All options of this app
		/// </summary>
		static public class Options
		{
			#region Setting Constants

			private const string HomePageKey = "opt_homepage";
			private const string EmulationKey = "opt_emulation";
			private const string AcceptCodeKey = "opt_accept_code";
			private const string AcceptEANKey = "opt_accept_ean";
			private const string AcceptUPCKey = "opt_accept_upc";
			private const string LockPortraitKey = "opt_portrait";
			private const string SaveHistoryKey = "opt_history";
			private const string UseLocationyKey = "opt_location";
			private const string LanguageKey = "opt_language";

			#endregion

			public static string HomePage                       // start page
			{
				get { return AppSettings.GetValueOrDefault<string>(HomePageKey, "https://adrianotiger.github.io/qr2web/scan.html"); }
				set { AppSettings.AddOrUpdateValue<string>(HomePageKey, value); }
			}

			public static int Emulation                         // emulation used after parsing a code
			{
				get { return AppSettings.GetValueOrDefault<int>(EmulationKey, EmulationTypes.NORMAL); }
				set { AppSettings.AddOrUpdateValue<int>(EmulationKey, value); }
			}

			public static bool AcceptBarcode_Code				// if Code (39, 93 and 128) should be parse by the scanner
			{
				get { return AppSettings.GetValueOrDefault<bool>(AcceptCodeKey, false); }
				set { AppSettings.AddOrUpdateValue<bool>(AcceptCodeKey, value); }
			}

			public static bool AcceptBarcode_Ean				// if Ean (8, 13) should be parse by the scanner
			{
				get { return AppSettings.GetValueOrDefault<bool>(AcceptEANKey, false); }
				set { AppSettings.AddOrUpdateValue<bool>(AcceptEANKey, value); }
			}

			public static bool AcceptBarcode_Upc				// if UPC (A, E, EAN) should be parse by the scanner
			{
				get { return AppSettings.GetValueOrDefault<bool>(AcceptUPCKey, false); }
				set { AppSettings.AddOrUpdateValue<bool>(AcceptUPCKey, value); }
			}

			public static bool LockPortrait						// lock app in portrait mode
			{
				get { return AppSettings.GetValueOrDefault<bool>(LockPortraitKey, true); }
				set { AppSettings.AddOrUpdateValue<bool>(LockPortraitKey, value); }
			}

			public static bool SaveHistory                      // save scanned codes in a history
			{
				get { return AppSettings.GetValueOrDefault<bool>(SaveHistoryKey, true); }
				set { AppSettings.AddOrUpdateValue<bool>(SaveHistoryKey, value); }
			}

			public static bool UseLocation                      // if the location should be forwarded to webpage
			{
				get { return AppSettings.GetValueOrDefault<bool>(UseLocationyKey, false); }
				set { AppSettings.AddOrUpdateValue<bool>(UseLocationyKey, value); }
			}

			public static int LanguageIndex                     // language used for this app
			{
				get { return AppSettings.GetValueOrDefault<int>(LanguageKey, 0); }
				set { AppSettings.AddOrUpdateValue<int>(LanguageKey, value); }
			}
		}

		/// <summary>
		/// Temporary options to use after a scan is complete. Once the scan is complete the options will be reset.
		/// </summary>
		/// <remarks>
		/// Temporary options are used for emulated protocols like mocha and pic2shop. 
		/// If the app is started through this protocol, a temporary option is set. 
		/// Once the scan is over the app will try to use the emulated functionality and then it will reset the options.
		/// </remarks>
		static public class TemporaryOptions
		{
			static private string LookupPage = "";
			static private string LookupFunction = "";
			static private int Emulation = EmulationTypes.UNKNOWN;
			static private bool OptionsSet = false;
			static public void ResetOptions()
			{
				LookupPage = "";
				LookupFunction = "";
				Emulation = EmulationTypes.UNKNOWN;
				OptionsSet = false;
			}
			static public void SetLookup(string function = "", string page = "", int emulation=EmulationTypes.UNKNOWN)
			{
				LookupPage = page;
				LookupFunction = function;
				Emulation = emulation;
				OptionsSet = true;
			}
			static public bool HasOptions() { return OptionsSet; }
			static public string GetHomePage() { return LookupPage; }
			static public string GetFunction() { return LookupFunction; }
			static public int GetEmulation() { return Emulation; }

		}
		
		private static string History                     // code history as single string (to save it as parameter)
		{
			get { return AppSettings.GetValueOrDefault<string>("history", ""); }
			set { AppSettings.AddOrUpdateValue<string>("history", value); }
		}

		/// <summary>
		/// Load parsed codes history from local memory (saved as one string)
		/// </summary>
		/// <param name="savedHistory">reference to the history variable</param>
		static public void LoadHistory(ref List<KeyValuePair<DateTime, string>> savedHistory)
		{
			string[] sCode = History.Split('#');
			try
			{
				for (int i = 0; i < sCode.Length; i++)
				{
					if (sCode[i].IndexOf("$") > 0)
					{
						string[] sC = sCode[i].Split('$');
						DateTime dt = DateTime.Parse(sC[0]);
						savedHistory.Add(
							new KeyValuePair<DateTime, string>
							(
								dt,
								sC[1]
							)
						);
					}
				}
			}
			catch (Exception)
			{

			}
		}

		/// <summary>
		/// Save history list to local memory.
		/// </summary>
		/// <param name="newHistory">reference to the history valiable</param>
		static public void SaveHistory(ref List<KeyValuePair<DateTime, string>> newHistory)
		{
			string sCodes = "";
			for (int i = 0; i < Math.Min(newHistory.Count, 10); i++)
			{
				sCodes += newHistory[i].Key.ToString();
				sCodes += "$";
				sCodes += newHistory[i].Value;
				sCodes += "#";
			}

			History = sCodes;
		}

	}
}
