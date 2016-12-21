
Scan codes with the smartphones and forward the parsed code to a webpage.

### Why this project?
You find always more QR codes on the web and it is also easy to generate QR codes for own projects. Good smartphones (with Android or Windows Mobile) are able to scan a QR code directly from a webpage. Older smartphones and cheaper smartphones need an app because they are not able to access the camera from the webpage. So you need an app for each OS (=> store accounts, developer accounts, and $$$).

This app is a simple QR scanner that will start over a simple link on the web. Once the scan is over, the app will forward the code to your webpage. 

So you need to write a webpage and give this free app to your users. **You don't need to write any app for this functionality anymore**, only the webpage.

### On which smartphone does it works?
The project was made with Xamarin and ZXing library. So it works with Android (>4.1), Windows Mobile (10), Windows (10) and iOS (>7).
Because I don't have any developer account on Android and iOS, I can give only the Windows Store link and Android APK. Maybe in future someone will compile and upload this app on the store for Android and iOS.

### GitHub Structure
App source code is in the [App folder](https://github.com/Adrianotiger/qr2web/tree/master/App) 

Webpage Demos and codes are in the [Example folder](https://github.com/Adrianotiger/qr2web/tree/master/Examples)

### Credits
It was possible to create this app thanks to Microsoft, Xamarin and ZXing libraries.

App was developed with [Visual Studio 2015](https://www.visualstudio.com/)

using the [Xamarin NuGet extension](https://www.xamarin.com/)

and the [ZXing BarcodeScanner library](https://github.com/Redth/ZXing.Net.Mobile)

### License
You can use this code and change it, you can also create your own app.

You are not allowed to publish your app on the store adding a banner or asking money for the compiled app.

### How to use the app
See the [wiki](https://github.com/Adrianotiger/qr2web/wiki) section to get help with this app.

### How write a webpage
Check the Example folder and the [wiki](https://github.com/Adrianotiger/qr2web/wiki) section.
