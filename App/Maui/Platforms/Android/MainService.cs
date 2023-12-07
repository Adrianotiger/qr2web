using Android.App;
using Android.Content;
using Android.OS;
using Maui;
using System.Diagnostics.Metrics;


namespace qr2web.Platforms.Android
{
    [Service(Exported = true)]
    internal class MainService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();

            string NOTIFICATION_CHANNEL_ID = "ServiceChannel";
            NotificationChannel chan = new(NOTIFICATION_CHANNEL_ID, "Background Service", NotificationImportance.High);

            NotificationManager? manager = (NotificationManager?)GetSystemService(Context.NotificationService);
            manager?.CreateNotificationChannel(chan);

            var notificationBuilder = new AndroidX.Core.App.NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);

            var closeIntent = new Intent(this, typeof(MainActivity));
            closeIntent.SetAction("qr2web.CLOSE_APP");
            var closeIntent2 = PendingIntent.GetActivity(this, 0, closeIntent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            Notification notification = notificationBuilder
                .SetContentTitle("Qr2Web is running")
                .SetContentText("Click here to close app")
                .SetCategory(Notification.CategoryService)
                .SetContentIntent(closeIntent2)
                .SetSilent(true)
                .SetAutoCancel(true)
                .SetOngoing(true)
                .Build();

            StartForeground(100, notification);
        }

        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }
    }
}
