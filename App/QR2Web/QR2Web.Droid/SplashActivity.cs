using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

using AndroidX.AppCompat.App;
using System.Threading.Tasks;

namespace QR2Web.Droid
{
    [Activity(
        Theme = "@style/MyTheme.Splash", 
        MainLauncher = true, 
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop,
        NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

    }
}