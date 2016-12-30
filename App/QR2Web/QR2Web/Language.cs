using System;
using System.Collections.Generic;
using System.Text;

namespace QR2Web
{
	/// <summary>
	/// Language class. This is a static class, so it already exists and can be used from every other class.
	/// </summary>
	/// <remarks>
	/// I know it would be better to use a Resource. But the work is too much to set a new resource to every project.
	/// Because the texts are not so much and this file can be small and doesn't use much memory, I used this solution.
	/// </remarks>
	static class Language
    {
		// Dictionary array with the string for each language (array of languages), sentence key and sentence in the selected language
		static private Dictionary<string, string>[] LanguageString;
		// List with all possible languages
		static private List<string> LanguageList;
		// current language (list index)
		static private int CurrentLanguageIndex;

		/// <summary>
		/// Initialize the language class. It initialize automatically, the first time it will be used.
		/// </summary>
		static private void InitLanguage()
		{
			CurrentLanguageIndex = 0;				// default: english
			
			LanguageList = new List<string>();
			LanguageList.Add("en");					// 0: english
			LanguageList.Add("de");					// 1: german
			LanguageList.Add("it");					// 2: italian

			LanguageString = new Dictionary<string, string>[LanguageList.Count];
			for (int i = 0; i < LanguageList.Count; i++)
			{
				LanguageString[i] = new Dictionary<string, string>(64);
			}

			FillDictionary();						// add the language texts
		}

		/// <summary>
		/// Add ann array of texts (every language) to the dictionary
		/// </summary>
		/// <param name="languageKey">a short string as key for the sentence</param>
		/// <param name="languagesTexts">array of strings containing all translations of the string</param>
		static private void AddText(string languageKey, string[] languagesTexts)
		{
			for(int i=0;i<LanguageList.Count;i++)
			{
				LanguageString[i].Add(languageKey, languagesTexts[i]);
			}
		}

		/// <summary>
		/// Set the language index for the default language translation used in the app
		/// </summary>
		/// <param name="languageIndex">index of the language</param>
		/// <returns>true if the new language was set, false if the index was out of range</returns>
		/// <seealso cref="SetLanguage(string)"/>
		static public bool SetLanguage(int languageIndex)
		{
			if (LanguageList == null) InitLanguage();
			if (languageIndex >= 0 && languageIndex < LanguageList.Count)
				return SetLanguage(LanguageList[languageIndex]);
			else
				return false;
		}

		/// <summary>
		/// Set the language (2 chars) of the wanted language
		/// </summary>
		/// <param name="language">string (like "en" or "de" or "it") to set the default language</param>
		/// <returns>true if it was possible to set the language, false if it was not possible</returns>
		/// <seealso cref="SetLanguage(int)"/>
		static public bool SetLanguage(string language)
		{
			if (LanguageList == null) InitLanguage();
			if (LanguageList.Contains(language))
			{
				CurrentLanguageIndex = LanguageList.IndexOf(language);
				return true;
			}
			else if(language.Length > 2 && LanguageList.Contains(language.Substring(0,2)))
			{
				CurrentLanguageIndex = LanguageList.IndexOf(language.Substring(0, 2));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Get the translated sting for the current key.
		/// </summary>
		/// <param name="TextKey">Key used to define the translated text to return</param>
		/// <returns>string containing the translated text in the selected language. If the string doesn't exist, "Undefined Key" will be returned.</returns>
		static public String GetText(string TextKey)
		{
			if (LanguageList == null) InitLanguage();

			if (LanguageString[CurrentLanguageIndex].ContainsKey(TextKey))
			{
				return LanguageString[CurrentLanguageIndex][TextKey];
			}
			else
			{
				return "Undefined Key: " + TextKey;
			}
		}

		/// <summary>
		/// Fill the dictionary with all translations
		/// </summary>
		static private void FillDictionary()
		{
			AddText("AppTitle", new string[] { "QR to Web Inventory", "QR to Web Inventory", "QR to Web Inventory" });
			AddText("AppTitleShort", new string[] { "QR2Web", "QR2Web", "QR2Web" });
			AddText("Settings", new string[] { "Settings", "Einstellungen", "Impostazioni" });
			AddText("Help", new string[] { "Help", "Hilfe", "Aiuto" });
			AddText("About", new string[] { "About", "Über", "Info" });
			AddText("History", new string[] { "History", "Verlauf", "Cronologia" });
			AddText("Options", new string[] { "Options", "Opzionen", "Opzioni" });
			AddText("Cancel", new string[] { "Cancel", "Abbrechnen", "Annulla" });
			AddText("ScannerText1", new string[] { "Scan with a distance of 20-30cm", "Abstand von 20-30cm halten", "Leggere da una distanza di 20-30cm" });
			AddText("ScannerText2", new string[] { "Light On/Off", "Licht Ein/Aus", "Luce On/Off" });
			AddText("NeedRestart", new string[] { "(* App must be restarted)", "(* App muss neu gestartet werden)", "(* necessita un riavvio dell'app)" });
			AddText("RefreshPage", new string[] { "Refresh page", "Seite aktualisieren", "Aggiorna pagina" });
			AddText("HomePage", new string[] { "Return to home page", "Zurück zur home page", "Torna alla pagina principale" });
			AddText("TimeToScan", new string[] { "(XX seconds to scan..)", "(XX Sekunden um zu scannen...)", "(XX secondi per scannerizzare...)" });

			AddText("Option1_1", new string[] { "Home Page", "Home Page", "Home Page" });
			AddText("Option2_1", new string[] { "Emulation", "Emulation", "Emulazione" });
			AddText("Option2_2", new string[] { "Disabled", "Deaktiviert", "Disattivato" });
			AddText("Option3_1", new string[] { "Accepted Barcodes", "Akzeptierte Barcodes", "Codici a barre accettati" });
			AddText("Option4_1", new string[] { "Lock portrait mode", "Auf Portrait Modus aufzwingen", "Blocca in modalità verticale" });
			AddText("Option5_1", new string[] { "Save history", "Verlauf aufzeichnen", "Salva cronologia" });
			AddText("Option6_1", new string[] { "Language", "Sprache", "Lingua" });
			AddText("Option6_2", new string[] { "English", "Englisch", "Inglese" });
			AddText("Option6_3", new string[] { "German", "Deutsch", "Tedesco" });
			AddText("Option6_4", new string[] { "Italian", "Italienisch", "Italiano" });
		}

    }
}
