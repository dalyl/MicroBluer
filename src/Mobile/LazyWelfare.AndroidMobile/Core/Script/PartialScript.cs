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

namespace LazyWelfare.AndroidMobile.Script
{
    public class PartialScript : AndroidScript//注意一定要继承java基类  
    {
        public WebPartialActivity PartialActivity
        {
            get
            {
                return ViewActivity as WebPartialActivity;
            }
        }

        public PartialScript(WebPartialActivity activity) : base(activity)
        {
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
            PartialView view = PartialView.None;
            if (Enum.TryParse<PartialView>(url, out view) == false) return string.Empty;

            switch (view)
            {
                case PartialView.None: break;
                case PartialView.HomeView:
                    {
                        PartialActivity.RequestStack.Clear();
                        PartialActivity.RequestStack.Push(view, args);
                        break;
                    }
                default:
                    {
                        PartialActivity.RequestStack.Push(view, args);
                        break;
                    }
            }

            switch (view)
            {
                case PartialView.HomeView: return WebViews.Get(url);
                case PartialView.HostsView:
                    {
                        var service = new HostStoreService(ViewActivity);
                        var model = service.GetList();
                        return WebViews.Get(url, model);
                    }
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