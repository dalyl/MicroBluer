using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.AndroidMobile.Models;
using LazyWelfare.AndroidMobile.Views;
using LazyWelfare.AndroidMobile.Views.Partials;
using System;

namespace LazyWelfare.AndroidMobile.Script
{
    public class PartialScript : AndroidScript//注意一定要继承java基类  
    {

        public PartialScript(WebPartialActivity activity, WebView brower) : base(activity, brower) { }

        [Export("RequestPartial")]
        [JavascriptInterface]
        public void RequestPartial(string frame, string type, string host, string url, string args,string after)
        {
            var context = new PartialRequestContext(frame, type, host, url, args, after);
            WebPartialLoadingAsyncTask loading = new WebPartialLoadingAsyncTask(ViewActivity as WebPartialActivity, context);
            loading.Execute();
        }

        void ShowLoading(Action Call)
        {
            Call?.Invoke();
        }

        void HideLoading(Action Call)
        {
            Call?.Invoke();
        }
    }


}