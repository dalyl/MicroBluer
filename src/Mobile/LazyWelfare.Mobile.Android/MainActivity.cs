using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using LazyWelfare.Mobile.Android.Views;
using LazyWelfare.Mobile.Android.Models;
using System.Collections.Generic;

namespace LazyWelfare.Mobile.Android
{
    [Activity(Label = "懒人触手", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;

            SetStartPage(webView);
        }


        void SetStartPage(WebView webView)
        {
            // Use subclassed WebViewClient to intercept hybrid native calls
            webView.SetWebViewClient(new HybridWebViewClient());

            var model = new Doll { Name = "ssssss" };
            // Render the view from the type generated from RazorView.cshtml
            var models = new List<Doll>() { model };
            var template = new DollList() { Model = models };
            var page = template.GenerateString();

            // Load the rendered HTML into the view with a base URL 
            // that points to the root of the bundled Assets folder
            webView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);
        }

    }


    public class ViewClient : WebViewClient
    {


        public override bool ShouldOverrideUrlLoading(WebView webView, string url)
        {

            var index = url.IndexOf(":");
            var scheme= url.Substring(index);
            switch (scheme)
            {
                case "": return true;
                default: return false;
            }
        }

    }

    public class HybridWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView webView, string url)
        {

            // If the URL is not our own custom scheme, just let the webView load the URL as usual
            var scheme = "hybrid:";

            if (!url.StartsWith(scheme))
                return false;

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

            return true;
        }
    }
}

