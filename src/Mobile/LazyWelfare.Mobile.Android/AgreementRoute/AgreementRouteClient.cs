using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace LazyWelfare.Mobile.Android
{
    public class AgreementRouteClient : WebViewClient
    {

        public override bool ShouldOverrideUrlLoading(WebView webView, string url)
        {
            var index = url.IndexOf(":");
            var tag = url.Substring(index);
            var scheme = Agreement.none;
            if (Enum.TryParse(tag, out scheme) == false) return false;
            switch (scheme)
            {
                case Agreement.scan: return ThrowHandle<HybridAgreement>(webView, url);
                case Agreement.hybrid: return ThrowHandle<HybridAgreement>(webView, url);
                default: return false;
            }
        }
        

        bool ThrowHandle<T>(WebView webView, string url) where T : IAgreementHandle, new()
        {
            var handler = new T();
            handler.Init(webView, url);
            return new T().Result;
        }

    }

}