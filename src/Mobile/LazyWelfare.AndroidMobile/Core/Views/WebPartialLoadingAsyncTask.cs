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
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.AndroidMobile.Utils;
using LazyWelfare.AndroidMobile.Views;

namespace LazyWelfare.AndroidMobile.Views
{
    public class WebPartialLoadingAsyncTask : AsyncTask
    {
        public static string EXTRA_ASYNCTASK_PARTIALREQUESTCONTEXT { get; } = BundleUtils.BuildKey<WebPartialLoadingAsyncTask>("EXTRA_ASYNCTASK_PARTIALREQUESTCONTEXT");

       

        WebPartialActivity _activity { get; }


        WebView _brower { get; set; }

        PartialRequestContext _context { get;  }

        string _content { get; set; }

        public WebPartialLoadingAsyncTask(WebPartialActivity viewActivity, PartialRequestContext context)
        {
            _activity = viewActivity;
           
            _brower = _activity.PartialView;
            _context = context;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            switch (_context.Host)
            {
                case nameof(PartialView):
                    _content = ActiveContext.Try.Invoke(string.Empty, () => PartialView.SwitchWebView(_activity as WebPartialActivity, _context.Url, _context.Args));
                    break;
                case nameof(ServiceHost):
                    _content = ServiceHost.PageDispatch(_context.Url, _context.Args);
                    break;

            }
            return true;
        }

        protected override void OnPreExecute()
        {
            base.OnPreExecute();
            _activity.ShowMaskLayer();

            //new AlertDialog.Builder(_activity)
            //   .SetTitle("Partial Loading")
            //   .SetMessage("Start!")
            //   .Show();
        }

        void Response()
        {
            _content = System.Net.WebUtility.HtmlEncode(_content).Replace("\r", " ").Replace("\n", " ");
            switch (_context.Form)
            {
                case PartialLoadForm.Replace:
                    _brower.EvaluateJavascript($"ViewScript.ReplacePartial('{_context.Frame}','{_content}')", null);
                    break;
                case PartialLoadForm.Append:
                    _brower.EvaluateJavascript($"ViewScript.AppendPartial('{_context.Frame}','{_content}')", null);
                    break;
            }
            if (string.IsNullOrEmpty(_context.After) == false)
            {
                _brower.EvaluateJavascript(_context.After, null);
            }
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            base.OnPostExecute(result);
            Response();
            _activity.HideMaskLayer();

            //new AlertDialog.Builder(_activity)
            //    .SetTitle("Partial Loading")
            //    .SetMessage("Over!")
            //    .Show();
        }
    }
}