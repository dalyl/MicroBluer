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
            if (invoke.Result)
            {
                var service = new HostStoreService(ViewActivity);
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
            Toast.MakeText(ViewActivity, Message, ToastLength.Short).Show();
        }


        [Export("PartialLoad")]
        [JavascriptInterface]
        public string PartialLoad(string host, string url, string args)
        {
            if (host == nameof(WebViews)) return SwitchWebView(url, args);
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

        string SwitchWebView(string url, string args)
        {
            var view = Enum.Parse(typeof(PartialView), url);
            switch (view)
            {
                case PartialView.HomeView: return WebViews.Get(url);
                case PartialView.HostsView:
                    {
                        var service = new HostStoreService(ViewActivity);
                        var list = service.GetList();
                        return WebViews.Get(url, list);
                    };
                case PartialView.HostDetailView:
                    {
                        var service = new HostStoreService(ViewActivity);
                        var model = string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : service.GetModel(args);
                        return WebViews.Get(url, model);
                    }
            }
            return string.Empty;
        }
    }


}