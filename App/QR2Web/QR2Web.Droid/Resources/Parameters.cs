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
			static public string HomePage { get; set; }			// callback home page - page to open when the app starts
			static public int Emulation { get; set; }			// emulation used after parsing a code
			static public bool AcceptBarcode_Code { get; set; }	// if Code (39, 93 and 128) should be parse by the scanner
			static public bool AcceptBarcode_Ean { get; set; }  // if Ean (8, 13) should be parse by the scanner
			static public bool AcceptBarcode_Upc { get; set; }  // if UPC (A, E, EAN) should be parse by the scanner
			static public bool LockPortrait { get; set; }		// lock app in portrait mode
			static public bool SaveHistory { get; set; }        // save scanned codes in a history
			static public bool UseLocation { get; set; }        // if the location should be forwarded to webpage
			static public int LanguageIndex { get; set; }		// language used for this app
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

		static private string History { get; set; } = "";		// code history as single string (to save it as parameter)
		
		/// <summary>
		/// Get the value of a parameter. If the parameter doesn't exists, use the default value.
		/// </summary>
		/// <param name="valueKey">key of the saved parameter</param>
		/// <param name="defaultValue">default value, if the parameter can't be found</param>
		/// <returns>string containing the parameter value</returns>
		static public string GetSavedValue(string valueKey, string defaultValue = "")
		{
			object retValue;
			if (Application.Current.Properties.TryGetValue(valueKey, out retValue))
			{
				if (retValue is string) return (string)retValue;
			}
			return defaultValue;
		}

		/// <summary>
		/// Load all parameters from memory and copy them to the member variables of this class.
		/// Default values of each parameter are set here.
		/// </summary>
		static public void LoadOptions()
		{
			Options.HomePage = GetSavedValue("opt_homepage", "https://adrianotiger.github.io/qr2web/scan.html");
			Options.Emulation = int.Parse(GetSavedValue("opt_emulation", EmulationTypes.NORMAL.ToString()));
			Options.AcceptBarcode_Code = bool.Parse(GetSavedValue("opt_accept_code", "false"));
			Options.AcceptBarcode_Ean = bool.Parse(GetSavedValue("opt_accept_ean", "false"));
			Options.AcceptBarcode_Upc = bool.Parse(GetSavedValue("opt_accept_upc", "false"));
			Options.LockPortrait = bool.Parse(GetSavedValue("opt_portrait", "true"));
			Options.SaveHistory = bool.Parse(GetSavedValue("opt_history", "true"));
			Options.LanguageIndex = int.Parse(GetSavedValue("opt_language", "0"));
			Options.UseLocation = bool.Parse(GetSavedValue("opt_location", "false"));
		}

		/// <summary>
		/// Load parsed codes history from local memory (saved as one string)
		/// </summary>
		/// <param name="History">reference to the history variable</param>
		static public void LoadHistory(ref List<KeyValuePair<DateTime, string>> History)
		{
			object ScannedCodes;
			if (Application.Current.Properties.TryGetValue("history", out ScannedCodes))
			{
				if (ScannedCodes is string)
				{
					string sCodes = (string)ScannedCodes;
					string[] sCode = sCodes.Split('#');
					try
					{
						for (int i = 0; i < sCode.Length; i++)
						{
							if (sCode[i].IndexOf("$") > 0)
							{
								string[] sC = sCode[i].Split('$');
								DateTime dt = DateTime.Parse(sC[0]);
								History.Add(
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
			}
		}

		/// <summary>
		/// Save history list to local memory.
		/// </summary>
		/// <param name="History">reference to the history valiable</param>
		static public void SaveHistory(ref List<KeyValuePair<DateTime, string>> History)
		{
			string sCodes = "";
			for (int i = 0; i < Math.Min(History.Count, 10); i++)
			{
				sCodes += History[i].Key.ToString();
				sCodes += "$";
				sCodes += History[i].Value;
				sCodes += "#";
			}
			Application.Current.Properties["history"] = sCodes;

			SaveParams();
		}
		
		/// <summary>
		/// Save parameters (need to save every time an option was changed)
		/// </summary>
		static public async void SaveParams()
		{
			Application.Current.Properties["opt_homepage"] = Options.HomePage.ToString();
			Application.Current.Properties["opt_emulation"] = Options.Emulation.ToString();
			Application.Current.Properties["opt_accept_code"] = Options.AcceptBarcode_Code.ToString();
			Application.Current.Properties["opt_accept_ean"] = Options.AcceptBarcode_Ean.ToString();
			Application.Current.Properties["opt_accept_upc"] = Options.AcceptBarcode_Upc.ToString();
			Application.Current.Properties["opt_portrait"] = Options.LockPortrait.ToString();
			Application.Current.Properties["opt_history"] = Options.SaveHistory.ToString();
			Application.Current.Properties["opt_language"] = Options.LanguageIndex.ToString();
			Application.Current.Properties["opt_location"] = Options.UseLocation.ToString();

			await Application.Current.SavePropertiesAsync();
		}
	}
}
