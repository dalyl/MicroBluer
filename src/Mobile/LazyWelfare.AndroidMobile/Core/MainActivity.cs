using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using LazyWelfare.AndroidMobile.Models;
using System.Collections.Generic;
using LazyWelfare.Common;
using Android.Content;
using System;
using LazyWelfare.AndroidMobile.Views;
using Android.Runtime;
using Android.Views;
using LazyWelfare.AndroidMobile.Views.Partials;

namespace LazyWelfare.AndroidMobile
{

    //LazyWelfare.AndroidMobile
    [Activity(Label = "@string/app_name", Icon = "@drawable/blue_face", Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {

        static (string Host, string Path) Partial;

        public MainActivity()
        {
            Partial = HomeView.Partial;
        }

        public MainActivity((string Host, string Path) view)
        {
            Partial = view;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var webView = FindViewById<WebView>(Resource.Id.webView);

            WebSettings settings = webView.Settings;
            //启用js事件  
            settings.SetSupportZoom(true);
            settings.JavaScriptEnabled = true;
            //启用js的dom缓存  
            settings.DomStorageEnabled = true;
            //加载javascript接口方法，以便调用前台方法  
            webView.AddJavascriptInterface(new AndroidScript(this), "AndroidScript");

            webView.SetWebViewClient(new AgreementRouteClient($"ViewScript.PartialLoad('#MainContent','{Partial.Host}','{Partial.Path}');"));

            var page = Template.Layout();
            webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);
        }

        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
            {
                if (!lastBackKeyDownTime.HasValue || DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 2))
                {
                    Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
                    lastBackKeyDownTime = DateTime.Now;
                }
                else
                {
                    Intent intent = new Intent();
                    intent.SetClass(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

    }



}