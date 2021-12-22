using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;

namespace QR2Web
{
    static class QRLocation
    {
        private static Position CurrentPosition;

        public static async void InitLocation(int accuracyInMeters = 100, bool updateLocationInBackground = true)
        {
            CrossGeolocator.Current.DesiredAccuracy = accuracyInMeters;

            if (!CrossGeolocator.Current.IsListening)
            {
                await StartListening();
            }
        }

        private static async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            if (!IsLocationAvailableOnDevice())
            {
                return;
            }
            if(!IsLocationEnabledOnDevice())
            {
                await App.Instance.MainPage.DisplayAlert("No GPS", "Location is disabled on your device, please activate it to use this functionality.", "OK");
                return;
            }

            try
            {
                var success = await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(30), 500, true);

                if (success)
                {
                    CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                    CrossGeolocator.Current.PositionError += Current_PositionError; ;
                }
            }
            catch (Exception)
            {

            }
        }

        private static void Current_PositionError(object sender, PositionErrorEventArgs e)
        {
            Console.WriteLine("Error reading position: " + e.Error);
        }

        private static void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            CurrentPosition = e.Position;
            string output = "Full: Lat: " + CurrentPosition.Latitude + " Long: " + CurrentPosition.Longitude;
            output += "\n" + $"Time: {CurrentPosition.Timestamp}";
            output += "\n" + $"Heading: {CurrentPosition.Heading}";
            output += "\n" + $"Speed: {CurrentPosition.Speed}";
            output += "\n" + $"Accuracy: {CurrentPosition.Accuracy}";
            output += "\n" + $"Altitude: {CurrentPosition.Altitude}";
            output += "\n" + $"Altitude Accuracy: {CurrentPosition.AltitudeAccuracy}";
            Console.WriteLine(output);
        }


        public static bool IsLocationAvailableOnDevice()
        {
            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        public static bool IsLocationEnabledOnDevice()
        {
            return CrossGeolocator.Current.IsGeolocationEnabled;
        }

        public static Position GetPosition()
        {
            return CurrentPosition;
        }

        public static string GenerateJavascriptString()
        {
            Position pos = GetPosition();
            string json = "";
            if (pos != null)
            {
                json += "{";
                json += " \"latitude\" : \"" + pos.Latitude.ToString().Replace(",", ".") + "\", ";
                json += " \"longitude\" : \"" + pos.Longitude.ToString().Replace(",", ".") + "\", ";
                json += " \"accuracy\" : \"" + pos.Accuracy.ToString().Replace(",", ".") + "\", ";
                json += " \"timestamp\" : \"" + pos.Timestamp.ToUnixTimeSeconds() + "\" ";
                json += "}";
            }
            else
            {
                json += "{\"latitude\":\"0.0\", \"longitude\":\"0.0\", \"accuracy\":\"10000.0\",\"timestamp\":\"0\"}";
            }

            string jscall = "";
            jscall = "window.setTimeout(function() {";
            jscall += "try {";
            jscall += "   if (\"function\" === typeof onQR2WebLocation){";
            jscall += "       onQR2WebLocation('" + json + "');}";
            jscall += "}catch(e){}}, 100);";

            return jscall;
        }
    }
}
