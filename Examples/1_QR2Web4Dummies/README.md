#QR2Web for dummies
This is a simple HTML page.
To test this page, you can open this link inside your app:
https://github.com/Adrianotiger/qr2web/blob/master/Examples/1_QR2Web4Dummies/index.html

Or set this link as webpage in your app (under setting) and open this link [qr2web:/scan](qr2web:/scan):
```
https://github.com/Adrianotiger/qr2web/blob/master/Examples/1_QR2Web4Dummies/index.html
```

The code of this page is really simple:
```
<html>
<head>
  <script>
  function onscan(barcode)
  {
    document.getElementById("scanResult").innerHTML = barcode;
  }
  </script>
</head>
<body>
  Click on the SCAN button in your app or open this link: <a href="qr2web:/scan">qr2web:/scan</a>
  <div id="scanResult">Scan result</div>
</body>
</html>
```

qr2web scheme allows you to give some parameters to the app.
With this example you can simply write `qr2web:/scan` or `qr2web:/scan/`. The scanner will scan the code and call the function "onscan()" inside your webpage. If you don't have this function, the page will not be able to get the scanned code.
