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
    public class ScanAgreement
    {
        public bool Result { get; private set; } = false;

        public void Init(WebView webView, string url)
        {
            webView.Context.StartActivity(typeof(ScanActivity));
            Result = true;
        }
    }
}