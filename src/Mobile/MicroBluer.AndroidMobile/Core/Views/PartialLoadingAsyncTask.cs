namespace MicroBluer.AndroidMobile.Views
{
    using Android.Webkit;
    using System;

    public class PartialLoadingAsyncTask : PartialBackgroudWorkerAsyncTask
    {

        WebView _brower { get; set; }

        PartialRequestContext _context { get;  }

        string _content { get; set; }

        protected override Action _invoke => DoInBackground;

        protected override Action _after => Response;

        public PartialLoadingAsyncTask(PartialActivity viewActivity, PartialRequestContext context) : base(viewActivity)
        {
            _brower = _activity.PartialView;
            _context = context;
        }

        void  DoInBackground()
        {
            switch (_context.Host)
            {
                case nameof(PartialHost):
                    _content = ActiveContext.Try.Invoke(string.Empty, () => PartialHost.Dispatch(_activity as PartialActivity, _context.Url, _context.Args));
                    break;
            }
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
      
    }


}