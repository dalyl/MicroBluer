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

namespace MicroBluer.AndroidMobile
{

    public class HybridAgreement : IAgreementHandler
    {

        public bool Result { get; private set; } = false;

        public void Init(WebView webView, string url)
        {

            // If the URL is not our own custom scheme, just let the webView load the URL as usual
            var scheme = "hybrid:";

            if (!url.StartsWith(scheme)) return;

            // This handler will treat everything between the protocol and "?"
            // as the method name.  The querystring has all of the parameters.
            var resources = url.Substring(scheme.Length).Split('?');
            var method = resources[0];
            var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

            if (method == "UpdateLabel")
            {
                var textbox = parameters["textbox"];

                // Add some text to our string here so that we know something
                // happened on the native part of the round trip.
                var prepended = string.Format("C# says \"{0}\"", textbox);

                // Build some javascript using the C#-modified result
                var js = string.Format("SetLabelText('{0}');", prepended);

                webView.LoadUrl("javascript:" + js);
            }

          
        }
    }
}