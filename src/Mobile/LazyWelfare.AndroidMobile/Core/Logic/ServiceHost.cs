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
using Android.Widget;
using LazyWelfare.AndroidMobile.Models;
using Newtonsoft.Json;

namespace LazyWelfare.AndroidMobile.Logic
{
    public class ServiceHost
    {

        const string ServiceDefineAddress = "/Data/ServiceDefine";
        const string DataAddress = "/Data/";
        const string CommandAddress = "/Command/";
        const string PageAddress = "/Page/";

        public static string PageDispatch(string url, string args, TryCatch Try)
        {
            if (Enable == false) return Try.Show<string>(string.Empty,"未设置 活动 Host 服务");
            var addr = $"{CurrentHost.Address}{PageAddress}{url.Replace("-",string.Empty)}";
            return Try.Invoke(string.Empty, () => GetContent(addr));
            return @"  <li class=""list-group-item main-item"">
                            <span class=""glyphicon glyphicon-off"" aria-hidden=""true""></span> 开关
                        </li>
                        <li class=""list-group-item command-item"">
                            <span class=""command-item-button orangered "">关机</span><span class=""command-item-button red"">睡眠</span>
                        </li>
                        <li class=""list-group-item main-command-item"">
                            <span class=""glyphicon glyphicon-headphones"" aria-hidden=""true""></span>音量调节
                        </li>";
        }

        public static HostModel CurrentHost { get; set; }

        static HttpClient Client { get; set; }

        public static bool Enable => CurrentHost != null;

        public static void SetHost(HostModel model, TryCatch Try)
        {
            var can = Try.Invoke(false, () => initHost(model), "服务地址不可用");
            if (can == false) CurrentHost = null;
        }

        static bool initHost(HostModel model)
        {
            CurrentHost = model;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(CurrentHost.Address);
            return true;
        }

        public static HostModel GetServiceDefine(string host, TryCatch Try)
        {
            var addr = $"{host}{ServiceDefineAddress}";
            var content = Try.Invoke(string.Empty, () => GetContent(addr), "服务地址不正确或服务不可用");
            if (string.IsNullOrEmpty(content)) return null;
            return Try.Invoke(null, () => JsonConvert.DeserializeObject<HostModel>(content),"服务接口数据解析失败");
        }

        static string GetContent(string url)
        {
            var fetchResponse = Client.GetByteArrayAsync(url);
            Task.WaitAll(fetchResponse);
            return Encoding.Default.GetString(fetchResponse.Result);
        }

        static string GetContent(string host,string url)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(CurrentHost.Address);
            var fetchResponse = client.GetByteArrayAsync(url);
            Task.WaitAll(fetchResponse);
            return Encoding.Default.GetString(fetchResponse.Result);
        }

    }
}