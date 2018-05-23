using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.AndroidMobile.Models;
using LazyWelfare.AndroidMobile.Views;
using LazyWelfare.AndroidMobile.Views.Partials;

namespace LazyWelfare.AndroidMobile.Script
{
    public class PartialScript : AndroidScript//注意一定要继承java基类  
    {
        public PartialScript(WebPartialActivity activity) : base(activity)
        {
        }

        [Export("PartialLoad")]
        [JavascriptInterface]
        public string PartialLoad(string host, string url, string args)
        {
            switch (host)
            {
                case nameof(WebPartialViews):return WebPartialViews.SwitchWebView(ViewActivity as WebPartialActivity, url, args);
                case nameof(ServiceHost): return ServiceHost.PageDispatch(url, args, Try);
            }
            return string.Empty;
        }
       
    }

   
}