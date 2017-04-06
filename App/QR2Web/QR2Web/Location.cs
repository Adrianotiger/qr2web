using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;

namespace QR2Web
{
	static class QRLocation
    {
		static private IGeolocator locator = CrossGeolocator.Current;
		static private bool isUpdatingLocation = false;
		static private int unsuccessCount = 0;

		static public Position CurrentPosition;

		static public void InitLocation(int accuracyInMeters = 100, bool updateLocationInBackground = true)
		{
			locator.DesiredAccuracy = accuracyInMeters;
			locator.AllowsBackgroundUpdates = updateLocationInBackground;
			UpdatePosition();
		}

		static public bool IsLocationAvailableOnDevice()
		{
			return locator.IsGeolocationAvailable;
		}

		static public bool IsLocationEnabledOnDevice()
		{
			return locator.IsGeolocationEnabled;
		}

		static public async Task<Position> GetPosition()
		{
			if (isUpdatingLocation)
			{
				await Task.Delay(200);
			}
			else
			{
				UpdatePosition();
			}

			return CurrentPosition;
		}

		static public async void UpdatePosition()
		{
			if (!isUpdatingLocation)
			{
				bool Success = false;
				Position newPosition;
				isUpdatingLocation = true;
				try
				{
					newPosition = await locator.GetPositionAsync(timeoutMilliseconds: 20000);
					Success = true;
					CurrentPosition = newPosition;
					unsuccessCount = 0;
					isUpdatingLocation = false;
				}
				catch(Exception)
				{

				}

				if (!Success && unsuccessCount < 6)
				{
					UpdatePosition();
				}
			}
		}

		static public async Task<string> GenerateJavascriptString()
		{
			var pos = await GetPosition();
			var json = "";
			json += "{";
			json += " \"latitude\" : \"" + pos.Latitude + "\", ";
			json += " \"longitude\" : \"" + pos.Longitude + "\", ";
			json += " \"accuracy\" : \"" + pos.Accuracy + "\", ";
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
