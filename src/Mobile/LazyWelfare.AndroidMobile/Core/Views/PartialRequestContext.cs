using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile.Views
{
    public class PartialRequestContext
    {
        public PartialRequestContext(string frame, string type, string host, string url, string args, string after)
        {
            Frame = frame;
            Type = type;
            Host = host;
            Url = url;
            Args = args;
            After = after;
            System.Enum.TryParse(Type, out _form);
        }

        private PartialLoadForm _form;

        public PartialLoadForm Form => _form;

        public string Frame { get; private set; }
        public string Type { get; private set; }
        public string Host { get; private set; }
        public string Url { get; private set; }
        public string Args { get; private set; }
        public string After { get; private set; }
    }
}