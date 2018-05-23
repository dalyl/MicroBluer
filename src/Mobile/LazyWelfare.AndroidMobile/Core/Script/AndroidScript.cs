using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using LazyWelfare.AndroidMobile.Logic;
using Newtonsoft.Json;

namespace LazyWelfare.AndroidMobile.Script
{
  

    public class AndroidScript : Java.Lang.Object//注意一定要继承java基类  
    {

        protected TryCatchActivity ViewActivity = null;

        protected TryCatch Try => ViewActivity.Try;

        public AndroidScript(TryCatchActivity activity)
        {
            ViewActivity = activity;
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

    }


}