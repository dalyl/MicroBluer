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

        protected Activity ViewActivity = null;
        protected TryCatch Try { get; }

        public AndroidScript(Activity activity)
        {
            ViewActivity = activity;
            Try = new TryCatch(ShowToast);
        }


        [Export("ScanHost")]
        [JavascriptInterface]
        public void ScanHost()
        {
            var scan = new ScanPlugin(ViewActivity);
            var invoke = scan.Invoke();
            Task.WaitAll(invoke);
            if (invoke.Result==false)
            {
                ShowToast("已取消");
            }
            if (string.IsNullOrEmpty(scan.Result))
            {
                ShowToast("未识别");
            }
            else
            {
                var service = new HostStoreService(ViewActivity);
                service.Add(scan.Result);
            }
        }

        /// <summary>  
        /// 弹出有消息的提示框（有参无反）  
        /// </summary>  
        /// <param name="Message"></param>  
        [Export("ShowToast")]//这个是js调用的c#类中方法名  
        [JavascriptInterface]//表示这个Method是可以被js调用的  
        public void ShowToast(string Message)
        {
            Toast.MakeText(ViewActivity, Message, ToastLength.Short).Show();
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