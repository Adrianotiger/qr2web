using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace QR2Web
{
	public class Scanner
	{
		private bool isScanning = false;

		public Scanner()
		{
			
		}

		public async Task<ZXing.Result> StartScan()
		{
			var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
			//options.TryHarder = true;
			//options.TryInverted = true;
			//options.AutoRotate = true;
			options.PossibleFormats = new List<ZXing.BarcodeFormat>();
			options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
			if (Parameters.Options.AcceptBarcode_Code)
			{
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_39);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_93);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_128);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.CODABAR);
			}
			if (Parameters.Options.AcceptBarcode_Ean)
			{
				options.PossibleFormats.Add(ZXing.BarcodeFormat.EAN_8);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.EAN_13);
			}
			if (Parameters.Options.AcceptBarcode_Upc)
			{
				options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_A);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_E);
				options.PossibleFormats.Add(ZXing.BarcodeFormat.UPC_EAN_EXTENSION);
			}

			isScanning = true;
			
			int iSecondsToScan = 10;
			String BaseText = Language.GetText("AppTitle") + "\n" + Language.GetText("TimeToScan").Replace("XX", iSecondsToScan.ToString());

			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			scanner.BottomText = Language.GetText("ScannerText1");
			scanner.CancelButtonText = Language.GetText("Cancel");
			scanner.FlashButtonText = Language.GetText("ScannerText2");
			scanner.TopText = BaseText;	

			Device.StartTimer(new TimeSpan(0, 0, 1), () =>
			{
				iSecondsToScan--;

				if (!isScanning)
				{
					return false;
				}
				if (iSecondsToScan < 0)
				{
					scanner.Cancel();
					return false;
				}

				return true;
			}
			);

			ZXing.Result result = await scanner.Scan(options);
			
			isScanning = false;

			return result;
		}

		public static bool IsAppURL(string url)
		{
			if (url.StartsWith("mochabarcode:") ||
				url.StartsWith("readbarcode:") ||
				url.StartsWith("p2spro:") || 
				url.StartsWith("qr2web:"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public string GenerateJavascriptString(string scanCode)
		{
			string jscall = "";
			int Emulation = 0;
			if(Parameters.TemporaryOptions.HasOptions() && Parameters.TemporaryOptions.GetEmulation() > 0)
			{
				Emulation = Parameters.TemporaryOptions.GetEmulation();
			}
			else if(Parameters.Options.Emulation > 0)
			{
				Emulation = Parameters.Options.Emulation;
			}

			switch (Emulation)
			{
				case Parameters.EmulationTypes.MOCHASOFT: // mocha
					{
						if (Parameters.TemporaryOptions.HasOptions() && Parameters.TemporaryOptions.GetHomePage().Length > 5)
						{
							jscall = "window.setTimeout(function() {";
							jscall += "try {";
							jscall += "   window.open('" + Parameters.TemporaryOptions.GetHomePage();
							jscall += (Parameters.TemporaryOptions.GetHomePage().IndexOf('?') > 5 ? "&" : "?") + "BARCODE=" + scanCode + "', '_self');}";
							jscall += "}catch(e){alert(\"unable to open page\")}}, 100);";
						}
						else
						{
							jscall = "window.setTimeout(function() {";
							jscall += "try {";
							jscall += "   if (\"function\" === typeof onscan){";
							jscall += "        onscan('" + scanCode + "');}";
							jscall += "   else if(document.getElementById('barcodefield1') && document.getElementById('barcodefield1').type == 'input'){";
							jscall += "        document.getElementById('barcodefield1').value='" + scanCode + "';}";
							jscall += "}catch(e){alert(\"onscan not found\")}}, 100);";
						}
						break;
					}
				case Parameters.EmulationTypes.PIC2SHOP: // p2spro
					{
						if (Parameters.TemporaryOptions.HasOptions() && Parameters.TemporaryOptions.GetHomePage().Length > 5)
						{
							jscall = "window.setTimeout(function() {";
							jscall += "try {";
							jscall += "   window.open('" + Parameters.TemporaryOptions.GetHomePage().Replace("CODE", scanCode);
							jscall += "', '_self');}";
							jscall += "}catch(e){alert(\"unable to open page\")}}, 100);";
						}
						else if (Parameters.TemporaryOptions.HasOptions() && Parameters.TemporaryOptions.GetFunction().Length > 3)
						{
							jscall = "window.setTimeout(function() {";
							jscall += "try {";
							jscall += Parameters.TemporaryOptions.GetFunction().Replace("CODE", scanCode);
							jscall += "}";
							jscall += "}catch(e){alert(\"insertCodeFormat not found\")}}, 100);";
						}
						else
						{
							jscall = "window.setTimeout(function() {";
							jscall += "try {";
							jscall += "   if (\"function\" === typeof insertCodeFormat){";
							jscall += "       insertCodeFormat('" + scanCode + "');}";
							jscall += "}catch(e){alert(\"insertCodeFormat not found\")}}, 100);";
						}
						break;
					}
				case Parameters.EmulationTypes.NORMAL:
				case Parameters.EmulationTypes.UNKNOWN:
				default:
					{
						jscall = "window.setTimeout(function() {";
						jscall += "try {";
						jscall += "   if (\"function\" === typeof insertScannedCode){";
						jscall += "       insertScannedCode('" + scanCode + "');}";
						jscall += "   else if (\"function\" === typeof onscan){";
						jscall += "       onscan('" + scanCode + "');}";
						jscall += "}catch(e){alert(\"insertScannedCode or onscan not found\")}}, 100);";
						break;
					}
			}

			return jscall;
		}

	}
		
}
