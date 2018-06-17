using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using MicroBluer.AndroidMobile.Logic;
using MicroBluer.AndroidMobile.Models;
using MicroBluer.AndroidMobile.Views;
using MicroBluer.AndroidMobile.Views.Partials;
using System;

namespace MicroBluer.AndroidMobile.Script
{
    public class PartialScript : AndroidScript//注意一定要继承java基类  
    {

        public PartialScript(PartialActivity activity, WebView brower) : base(activity, brower) { }

        [Export("RequestPartial")]
        [JavascriptInterface]
        public void RequestPartial(string frame, string type, string host, string url, string args, string after)
        {
            var context = new PartialRequestContext(frame, type, host, url, args, after);
            var loading = new PartialLoadingAsyncTask(ViewActivity as PartialActivity, context);
            loading.Execute();
        }

    }


}