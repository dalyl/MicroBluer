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
using LazyWelfare.AndroidMobile.Script;
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.AndroidAreaView.Core;

namespace LazyWelfare.AndroidMobile
{


    //LazyWelfare.AndroidMobile
    [Activity(Label = "@string/app_name", Icon = "@drawable/blue_face", Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : WebPartialActivity
    {

        static (string Host, string Path) Partial;

        static WebPartialRequestStack requestStack { get; set; } = new WebPartialRequestStack();

        public override WebPartialRequestStack RequestStack { get; } = requestStack;

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


            var ctrl = new MapCtrlDialog(this);
            ctrl.Show();

            return;

            PartialView = FindViewById<WebView>(Resource.Id.webView);

            WebSettings settings = PartialView.Settings;
            
            //启用js事件  
            settings.SetSupportZoom(true);
            settings.JavaScriptEnabled = true;
            //启用js的dom缓存  
            settings.DomStorageEnabled = true;
            //加载javascript接口方法，以便调用前台方法  
            PartialView.AddJavascriptInterface(new AndroidScript(this, PartialView), "AndroidScript");
            PartialView.AddJavascriptInterface(new PartialScript(this, PartialView), "PartialScript");
            PartialView.AddJavascriptInterface(new BuinessScript(this, PartialView), "BuinessScript");

            PartialView.SetWebViewClient(new AgreementRouteClient($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{Partial.Host}','{Partial.Path}',null);"));

            var userService = new UserStoreService(this);
            var hostService   = new HostStoreService(this);

            var model = userService.Get();
            var host = string.IsNullOrEmpty(model.Host) ? null : hostService.Get(model.Host);
            ServiceHost.SetHost(host, Try);
            var page = Template.Layout(model.Name);
            PartialView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);

          
        }
       
    }



}