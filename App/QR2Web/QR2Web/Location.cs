using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;

namespace QR2Web
{
	static class QRLocation
    {
        static private Position CurrentPosition;

        static public async void InitLocation(int accuracyInMeters = 100, bool updateLocationInBackground = true)
		{
            CrossGeolocator.Current.DesiredAccuracy = accuracyInMeters;

            if (!CrossGeolocator.Current.IsListening)
            {
                await StartListening();
            }
        }

        static private async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            if (!IsLocationAvailableOnDevice() || !IsLocationEnabledOnDevice()) return;

            try
            {
                await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(30), 500, true);

                CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                CrossGeolocator.Current.PositionError += Current_PositionError; ;
            }
            catch(Exception)
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
            var output = "Full: Lat: " + CurrentPosition.Latitude + " Long: " + CurrentPosition.Longitude;
            output += "\n" + $"Time: {CurrentPosition.Timestamp}";
            output += "\n" + $"Heading: {CurrentPosition.Heading}";
            output += "\n" + $"Speed: {CurrentPosition.Speed}";
            output += "\n" + $"Accuracy: {CurrentPosition.Accuracy}";
            output += "\n" + $"Altitude: {CurrentPosition.Altitude}";
            output += "\n" + $"Altitude Accuracy: {CurrentPosition.AltitudeAccuracy}";
            Console.WriteLine(output);
        }


        static public bool IsLocationAvailableOnDevice()
		{
			return CrossGeolocator.Current.IsGeolocationAvailable;
		}

		static public bool IsLocationEnabledOnDevice()
		{
			return CrossGeolocator.Current.IsGeolocationEnabled;
		}

		static public Position GetPosition()
		{
			return CurrentPosition;
		}
        
		static public string GenerateJavascriptString()
		{
			var pos = GetPosition();
			var json = "";
			json += "{";
			json += " \"latitude\" : \"" + pos.Latitude.ToString().Replace(",", ".") + "\", ";
			json += " \"longitude\" : \"" + pos.Longitude.ToString().Replace(",", ".") + "\", ";
			json += " \"accuracy\" : \"" + pos.Accuracy.ToString().Replace(",", ".") + "\", ";
			json += " \"timestamp\" : \"" + pos.Timestamp.ToUnixTimeSeconds() + "\" ";
			json +=	"}";

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
