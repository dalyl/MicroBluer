using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using LazyWelfare.AndroidMobile.Models;
using System.Collections.Generic;
using LazyWelfare.Service;
using Android.Content;
using System;
using LazyWelfare.AndroidMobile.Views;

namespace LazyWelfare.AndroidMobile
{
    //LazyWelfare.AndroidMobile
    [Activity(Label = "@string/app_name", Icon = "@drawable/blue_face", Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;
            webView.SetWebViewClient(new AgreementRouteClient());
            var model = new HomeModel
            {
                Header = "Hello,你好",
            };
            var template = new HomeView() { Model = model };
            var page = template.GenerateString();
            webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);
        }
      
    }
    
}