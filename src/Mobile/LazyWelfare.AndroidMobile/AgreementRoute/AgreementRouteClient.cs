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

namespace LazyWelfare.AndroidMobile
{
    public class AgreementRouteClient : WebViewClient
    {

        public override bool ShouldOverrideUrlLoading(WebView webView, string url)
        {
            var index = url.IndexOf(":");
            var tag = url.Substring(0,index);
            var content = url.Substring(index + 1);
            var scheme = Agreement.none;
            if (Enum.TryParse(tag, out scheme) == false) return false;
            switch (scheme)
            {
                case Agreement.scan: return ThrowHandle<ScanAgreement>(webView, content);
                case Agreement.hybrid: return ThrowHandle<HybridAgreement>(webView, content);
                default: return false;
            }
        }
        

        bool ThrowHandle<T>(WebView webView, string url) where T : IAgreementHandler, new()
        {
            var handler = new T();
            handler.Init(webView, url);
            return new T().Result;
        }

    }

}