﻿namespace LazyWelfare.AndroidMobile.Views
{
    using Android.OS;
    using Android.Webkit;
    using LazyWelfare.AndroidUtils.Common;

    public class PartialLoadingAsyncTask : AsyncTask
    {
        public static string EXTRA_ASYNCTASK_PARTIALREQUESTCONTEXT { get; } = BundleUtils.BuildKey<PartialLoadingAsyncTask>("EXTRA_ASYNCTASK_PARTIALREQUESTCONTEXT");

        PartialActivity _activity { get; }

        WebView _brower { get; set; }

        PartialRequestContext _context { get;  }

        string _content { get; set; }

        public PartialLoadingAsyncTask(PartialActivity viewActivity, PartialRequestContext context)
        {
            _activity = viewActivity;
           
            _brower = _activity.PartialView;
            _context = context;
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            switch (_context.Host)
            {
                case nameof(PartialHost):
                    _content = ActiveContext.Try.Invoke(string.Empty, () => PartialHost.Dispatch(_activity as PartialActivity, _context.Url, _context.Args));
                    break;

            }
            return true;
        }

        protected override void OnPreExecute()
        {
            base.OnPreExecute();
            _activity.ShowMaskLayer();
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
        }
    }
}