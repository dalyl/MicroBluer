namespace MicroBluer.AndroidMobile.Script
{
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Java.Interop;
    using Android.Webkit;
    using Android.Widget;
    using MicroBluer.AndroidUtils;

    public class AndroidScript : Java.Lang.Object//注意一定要继承java基类  
    {

        protected ActiveActivity ViewActivity { get; }

        protected WebView WebBrower { get; }

        protected TryCatch Try => TryCatch.Current;

        public AndroidScript(ActiveActivity activity, WebView brower)
        {
            ViewActivity = activity;
            WebBrower = brower;
        }

        /// <summary>  
        /// 弹出有消息的提示框（有参无反）  
        /// </summary>  
        /// <param name="Message"></param>  
        [Export("ShowToast")]//这个是js调用的c#类中方法名  
        [JavascriptInterface]//表示这个Method是可以被js调用的  
        public void ShowToast(string Message)
        {
            Toast.MakeText(ViewActivity, Message.Trim(), ToastLength.Short).Show();
        }
      

        protected T DeserializeForm<T>(string args)
        {
            var code = WebUtility.UrlDecode(args);
            var items = code.Split('&');
            var json = new StringBuilder();
            json.Append("{");
            foreach (var it in items)
            {
                var part = it.Replace("=", ":'");
                json.Append($"{part}',");
            }
            json.Append("}");
            var result = json.ToString().Replace(",}", "}");
            return JsonConvert.DeserializeObject<T>(result);
        }

        public  void EvaluateJavascript( string script)
        {
            WebBrower.EvaluateJavascript(script, null);
        }

    }


}