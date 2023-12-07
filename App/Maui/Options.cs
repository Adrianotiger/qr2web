
using banditoth.MAUI.PreferencesExtension;
using Microsoft.Maui.Controls.Compatibility;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;

namespace qr2web
{
    internal class HistoryData
    {
        public Options.BarcodeType Type { get; set; } = Options.BarcodeType.Invalid;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Scan { get; set; } = "";
    }

    internal class Options
    {
        public enum BarcodeType
        {
            Invalid = 0,
            QR = 1,
            Code = 2,
            Ean = 3,
            Upc = 4,
            Other1d = 5,
            Keyboard = 20,
            History = 25
        };
        public enum KeyboardType
        {
            Overlay = 1,
            Numeric = 2,
            Plain = 3,
            Disabled = 0
        };
        // todo: add languages and location
        public enum Languages
        {
            System,
            English,
            Deutsch,
            Italiano
        };

        public static Dictionary<BarcodeType, bool> Barcodes { get; private set; } = [];
        public static string HomePage { get; private set; } = "https://adrianotiger.github.io/qr2web/";
        public static bool ForcePortrait { get; private set; } = true;
        public static bool ShowScanbutton { get; private set; } = true;
        public static bool UseHistory { get; private set; } = true;
        public static bool ForwardLocation { get; private set; } = false;
        public static bool ForceBackground {  get; private set; } = false;
        public static bool ExternalLinks { get; private set; } = true;
        public static string Language { get; private set; } = "sy";
        public static KeyboardType Keyboard { get; private set; } = KeyboardType.Overlay;

        // Protected Options
        internal static string ConfigurationFile { get; set; } = "";
        internal static bool ShowFullscreen { get; set; } = false;

        // Not addeded to Options (internal, without setter / not a property)
        internal static List<HistoryData> History = [];

        private readonly static Dictionary<string, string> AllOptions = [];
        private readonly static Dictionary<string, string> ProtectedOptions = [];

        public static void InitOptions()
        {
            foreach (MemberInfo field in typeof(Options).GetMembers(BindingFlags.Public | BindingFlags.Static))
            {
                if(field.MemberType == MemberTypes.Property)
                {
                    AllOptions.Add(field.Name, "option_" + field.Name.ToLower());
                }
            }

            foreach (MemberInfo field in typeof(Options).GetMembers(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field.MemberType == MemberTypes.Property)
                {
                    ProtectedOptions.Add(field.Name, "option_" + field.Name.ToLower());
                }
            }

            try
            {
                string acceptedBarcodes = Preferences.Get(AllOptions[nameof(Barcodes)], "qr");

                Enum.GetValues(typeof(BarcodeType)).Cast<BarcodeType>().ToList().ForEach(bct =>
                {
                    if((int)bct > 0 && (int)bct < 20)
                    {
                        Barcodes.Add(bct, acceptedBarcodes.Contains(bct.ToString(), StringComparison.CurrentCultureIgnoreCase));
                    }
                });

                HomePage = Preferences.Get(AllOptions[nameof(HomePage)], "https://adrianotiger.github.io/qr2web/");

                ForcePortrait = Preferences.Get(AllOptions[nameof(ForcePortrait)], ForcePortrait);
                ShowScanbutton = Preferences.Get(AllOptions[nameof(ShowScanbutton)], ShowScanbutton);
                UseHistory = Preferences.Get(AllOptions[nameof(UseHistory)], UseHistory);
                ForceBackground = Preferences.Get(AllOptions[nameof(ForceBackground)], ForceBackground);
                ForwardLocation = Preferences.Get(AllOptions[nameof(ForwardLocation)], ForwardLocation);
                ExternalLinks = Preferences.Get(AllOptions[nameof(ExternalLinks)], ExternalLinks);
                Keyboard = PreferencesExtension.GetObject(AllOptions[nameof(Keyboard)], Keyboard);
                Language = Preferences.Get(AllOptions[nameof(Language)], Language);

                UpdateAppCulture();
                

                History = PreferencesExtension.GetObject("history", History);
            }
            catch
            {
                History.Clear();
            }
        }

        private static void UpdateAppCulture(bool update = false)
        {
            if (Language != "sy")
            {
                try
                {
                    CultureInfo newCulture = new(Language);
                    CultureInfo.DefaultThreadCurrentCulture = newCulture;
                }
                catch
                {

                }
            }
            else if(update)
            {
                CultureInfo.DefaultThreadCurrentCulture = null;
            }
        }

        public static void SaveHomePage(string page)
        {
            SaveParam(nameof(HomePage), page);
            SaveParam(nameof(ConfigurationFile), "");
        }

        public static void SetForcePortrait(bool force) => SaveParam(nameof(ForcePortrait), force);

        public static void SetShowScanButton(bool show) => SaveParam(nameof(ShowScanbutton), show);
        
        public static void SetUseHistory(bool use) => SaveParam(nameof(UseHistory), use);
        
        public static void SetForceBackground(bool force) => SaveParam(nameof(ForceBackground), force);

        public static void SetExternalLinks(bool allow) => SaveParam(nameof(ExternalLinks), allow);

        public static void SetForwardLocation(bool forward) => SaveParam(nameof(ForwardLocation), forward);

        public static void SetKeyboardType(KeyboardType ktype) => SaveParam(nameof(Keyboard), ktype);

        public static void SetKeyboardType(string ktype)
        {
            Enum.GetValues(typeof(KeyboardType)).Cast<KeyboardType>().ToList().ForEach(kt =>
            {
                if (ktype == kt.ToString()) SetKeyboardType(kt);
            });
        }
        public static void SetLanguage(string lang)
        {
            lang = lang.ToLower();
            Enum.GetValues(typeof(Languages)).Cast<Languages>().ToList().ForEach(lng =>
            {
                if (lang.Equals(lng.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    lang.Equals(lng.ToString()[..2], StringComparison.CurrentCultureIgnoreCase))
                {
                    if(!lng.ToString().Equals(Language, StringComparison.CurrentCultureIgnoreCase))
                    {
                        UpdateAppCulture(true);
                    }
                    SaveParam(nameof(Language), lng.ToString().ToLower()[..2]);
                }
            });
        }

        public static void AddHistoryScan(HistoryData data)
        {
            if(!UseHistory)
                return;

            HistoryData? removeItem = null;
            History.ForEach(history =>
            {
                if(history.Scan == data.Scan)
                {
                    removeItem = history;
                }
            });
            if(removeItem != null) { History.Remove(removeItem); }
            History.Insert(0, data);

            PreferencesExtension.SetObject("history", History);
        }

        public static void ActivateBarcode(BarcodeType type, bool activate)
        {
            if (Barcodes[type] != activate)
            {
                Barcodes[type] = activate;
                List<string> newCodes = [];
                Barcodes.ToList().ForEach(bc =>
                {
                    if (bc.Value) newCodes.Add(bc.ToString().ToLower());
                });
                Preferences.Set(AllOptions[nameof(Barcodes)], String.Join(",", newCodes));
            }
        }

        public static bool CheckStringParam(string param, out string error, out Dictionary<string, string> keyValuePairs)
        {
            error = "";
            keyValuePairs = [];

            string[] list = param.Split('&');
            if (list.Length == 0)
            {
                error = "No valid parameters";
                return false;
            }
            
            foreach (var item in list)
            {
                bool paramExists = true;
                string[] pair = item.Split("=");
                if(pair.Length != 2)
                {
                    error = "Invalid parameter: " + item;
                    return false;
                }
                if (!AllOptions.ContainsValue("option_" + pair[0].ToLower()))
                {
                    if (!ProtectedOptions.ContainsKey("option_" + pair[0].ToLower()))
                    {
                        paramExists = false;
                        error += "Parameter '" + pair[0] + "' not found.\n";
                    }
                }
                if (paramExists)
                {
                    keyValuePairs.Add("option_" + pair[0].ToLower(), Uri.UnescapeDataString(pair[1].Trim()));
                }
            }

            return true;
        }

        public static bool UpdateNewParams(string param, out string error)
        {
            if(!CheckStringParam(param, out error, out Dictionary<string, string> keyValuePairs))
            {
                return false;
            }
            keyValuePairs.ToList().ForEach(kvp =>
            {
                PropertyInfo? pi = typeof(Options).GetProperty(kvp.Key);
                if(pi != null)
                {
                    if(pi.PropertyType == typeof(string)) 
                    { 
                    }
                    else if(pi.PropertyType == typeof(bool)) 
                    { 
                    }
                }
            });
            return true;
        }

        private static void SaveParam(string paramName, bool newValue)
        {
            PropertyInfo? pi = typeof(Options).GetProperty(paramName);
            if (pi != null && newValue != (bool?)pi.GetValue(true))
            {
                bool bVal = newValue;
                pi.SetValue(bVal, newValue);
                Preferences.Set(AllOptions[paramName], newValue);
            }
        }

        private static void SaveParam(string paramName, string newValue)
        {
            PropertyInfo? pi = typeof(Options).GetProperty(paramName);
            if (pi != null && newValue != (string?)pi.GetValue("-"))
            {
                string sVal = newValue;
                pi.SetValue(sVal, newValue);
                Preferences.Set(AllOptions[paramName], newValue);
            }
        }

        private static void SaveParam(string paramName, KeyboardType newValue)
        {
            PropertyInfo? pi = typeof(Options).GetProperty(paramName);
            if (pi != null && newValue != (KeyboardType?)pi.GetValue(KeyboardType.Overlay))
            {
                KeyboardType tVal = newValue;
                pi.SetValue(tVal, newValue);
                PreferencesExtension.SetObject(AllOptions[paramName], newValue);
            }
        }
    }
}
