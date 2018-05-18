using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.AndroidMobile.Models;
using LazyWelfare.AndroidMobile.Views;
using LazyWelfare.AndroidMobile.Views.Partials;
using ZXing;
using ZXing.Mobile;

namespace LazyWelfare.AndroidMobile
{
    public class AndroidScript : Java.Lang.Object//注意一定要继承java基类  
    {

        Activity _activity = null;
        public AndroidScript(Activity activity)
        {
            _activity = activity;
            Appcation.CurrentContext = _activity;
        }

        [Export("ScanHost")]
        [JavascriptInterface]
        public void ScanHost()
        {
            var scan = new ScanPlugin(_activity);
            var invoke = scan.Invoke();
            Task.WaitAll(invoke);
            if (invoke.Result)
            {
                var service = new HostStoreService();
                service.Add(scan.Result);
            }
            else
            {
                ShowToast("未识别");
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
            Toast.MakeText(_activity, Message, ToastLength.Short).Show();
        }

        [Export("PartialLoad")]
        [JavascriptInterface]
        public string PartialLoad(string host, string url)
        {
            if (host == nameof(WebViews)) return SwitchWebView(url);
            var ip = SwitchHost(host);
            if (string.IsNullOrEmpty(ip)) return string.Empty;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ip);
            var fetchResponse = client.GetByteArrayAsync(url);
            Task.WaitAll(fetchResponse);
            return Encoding.Default.GetString(fetchResponse.Result);
        }


        string SwitchHost(string host)
        {
            return "";
        }


        string SwitchWebView(string url)
        {
            var view = Enum.Parse(typeof(PartialView), url);
            switch (view) {
                case PartialView.HomeView: return WebViews.Get(url);
                case PartialView.HostsView: {
                        var service = new HostStoreService();
                        var list = service.GetList();
                        return WebViews.Get(url, list);
                    }
            }
            return "";
        }
    }


}