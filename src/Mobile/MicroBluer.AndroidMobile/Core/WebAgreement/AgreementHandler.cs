using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace MicroBluer.AndroidMobile
{
    public abstract  class AgreementHandler
    {
        public bool Result { get; private set; } = false;

        public Context Activity { get; private set; }

        public WebView WebBrower { get; private set; }

        public string RequestContent { get; private set; }

        public async void Init(WebView webView, string url)
        {
            WebBrower = webView;
            Activity = WebBrower.Context;
            RequestContent = url;
            await Invoke();
        }

        protected abstract Task Invoke();
    }
}