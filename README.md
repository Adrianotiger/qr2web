
Scan codes with the smartphones and forward the parsed code to a webpage.

<a href="https://github.com/Adrianotiger/qr2web/wiki/download">Download App directly from store:

<img src="https://github.com/Adrianotiger/qr2web/raw/gh-pages/images/stores.png">
</a>

### What is QR2Web?
QR to web is a simple barcodescanner with integrated browser.
The webpage can interact with the browser to start a barcode scanner or to light up the flashlight of your phone.

The app is useful if you are a webpage developer and want to <b>integrate a barcode scanner in your webpage</b>.

<img src="https://github.com/Adrianotiger/qr2web/blob/b745516fedebe9faf29e6c8229ec0c7e50e87c4a/screenshots/Screenshot_2021-12-20-15-54-23-635_ch.petrucci.qr2web_2.jpg" width="200px"> <img src="https://github.com/Adrianotiger/qr2web/raw/b745516fedebe9faf29e6c8229ec0c7e50e87c4a/screenshots/Screenshot_2021-12-20-15-55-37-865_ch.petrucci.qr2web_2.jpg" width="200px">

### Why this project?
You find always more QR codes on the web and it is also easy to generate QR codes for own projects. Good smartphones (with Android or Windows Mobile) are able to scan a QR code directly from a webpage. Older smartphones and cheaper smartphones need an app because they are not able to access the camera from the webpage. So you need an app for each OS (=> store accounts, developer accounts, and $$$).

This app is a simple QR scanner that will start over a simple link on the web. Once the scan is over, the app will forward the code to your webpage. 

So you need to write a webpage and give this free app to your users. **You don't need to write any app for this functionality anymore**, only the webpage.

### On which smartphone does it works?
The project was made with Xamarin and ZXing library. So it works with Android (>4.1), Windows (10) and iOS (>7).
Because I don't have any developer account on iOS, I can give only the Windows Store link and Android Play link. Maybe in future someone will compile and upload this app on the store for iOS.

### GitHub Structure
App source code is in the [App folder](https://github.com/Adrianotiger/qr2web/tree/master/App) 

Webpage Demos and codes are in the [Example folder](https://github.com/Adrianotiger/qr2web/tree/master/Examples)

Dedicated webpage is in the [Home Page](https://adrianotiger.github.io/qr2web/) of this project

There is also a  [Blog](https://adrianotiger.github.io/qr2web/blog) for future updates

### Credits
It was possible to create this app thanks to Microsoft, Xamarin and ZXing libraries.

App was developed with [Visual Studio 2019](https://www.visualstudio.com/)

using the [Xamarin NuGet extension](https://www.xamarin.com/),

the [ZXing BarcodeScanner library](https://github.com/Redth/ZXing.Net.Mobile)

and the [Geolocator Plugin](https://github.com/jamesmontemagno/xamarin.plugins)

### License
You can use this code and change it, you can also create your own app.

You are not allowed to publish your app on the store adding a banner or asking money for the compiled app.

### How to use the app
See the [wiki](https://github.com/Adrianotiger/qr2web/wiki) section to get help with this app.

### How write a webpage
Check the Example folder and the [wiki](https://github.com/Adrianotiger/qr2web/wiki) section.
