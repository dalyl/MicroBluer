using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.ImageSelect.Engine;

namespace LazyWelfare.AndroidMobile
{
    [Activity(MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash",
      ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : Activity
    {
        public static int REQUEST_CODE_CHOOSE = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //var intent = new Intent(this, typeof(ImageSelectActivity));
            //StartActivity(intent);

            var pick = Picker.From(this);
            pick.Count(3);
            pick.EnableCamera(true);
            //.SetEngine(new GlideEngine())
            //                .setEngine(new PicassoEngine())
            //                .setEngine(new ImageLoaderEngine())
            pick.SetEngine(new CustomEngine());
            pick.ForResult(REQUEST_CODE_CHOOSE);

          //  Finish();
        }
    }
}