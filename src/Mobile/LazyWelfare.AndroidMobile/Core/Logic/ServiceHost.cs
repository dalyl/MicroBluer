namespace LazyWelfare.AndroidMobile.Logic
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Android.Content;
    using LazyWelfare.AndroidMobile.AgreementServices;
    using LazyWelfare.AndroidMobile.Models;
    using Newtonsoft.Json;

    public class ServiceHost
    {

        const string ServiceDefineAddress = "/Data/ServiceDefine";
        const string DataAddress = "/Data/";
        const string CommandAddress = "/Command/";
        const string PageAddress = "/Page/";

        public static HostModel CurrentHost { get; set; }

        static HttpClient Client { get; set; }

        public static bool Enable => CurrentHost != null;

        public static void SetHost(HostModel model)
        {
            var can = ActiveContext.Try.Invoke(false, () => InitHost(model), "服务地址不可用");
            if (can == false) CurrentHost = null;
        }


        static bool InitHost(HostModel model)
        {
            CurrentHost = model;
            Client = CreateClient(CurrentHost.Address);
            return true;
        }

        public static string PageDispatch(string url, string args)
        {
            if (Enable == false) return ActiveContext.Try.Show<string>(string.Empty, "Host 服务不可用");
            return ActiveContext.Try.Invoke(string.Empty, () => GetPageContent(url));
        }

        public static HostModel GetServiceDefine(string host)
        {
            var content = ActiveContext.Try.Invoke(string.Empty, () => GetHostService(host, ServiceDefineAddress), "服务地址不正确或服务不可用");
            if (string.IsNullOrEmpty(content)) return null;
            return ActiveContext.Try.Invoke(null, () => JsonConvert.DeserializeObject<HostModel>(content),"服务接口数据解析失败");
        }

        public static bool InvokeCommand(Argument arg, Context context)
        {
            if (Enable == false) return ActiveContext.Try.Show(false, "Host 服务不可用");
            if (AgreementService.Contains(arg.Service)) return ActiveContext.Try.Invoke<bool>(false, () => AgreementService.Execute(context));
            if (string.IsNullOrEmpty(arg.Uri)) return ActiveContext.Try.Show(false, $"{arg.Name.Trim()}服务地址未提供");
            var result = ActiveContext.Try.Invoke(false, () => SendCommand(arg.Uri));
            if (result == false) ActiveContext.Try.Show(false, $"{arg.Name} 命令执行失败");
            return result;
        }

        internal static bool SendCommand(string cmd)
        {
            if (Enable == false) return false;
            var url = $"{CommandAddress}{cmd}";
            var fetchResponse = Client.GetAsync(url);
            return true;
        }

        internal static string GetCommandResult(string cmd)
        {
            var url = $"{CommandAddress}{cmd}";
            var fetchResponse = Client.GetByteArrayAsync(url);
            Task.WaitAll(fetchResponse);
            return Encoding.Default.GetString(fetchResponse.Result);
        }

        static string GetPageContent(string page)
        {
            var url = $"{PageAddress}{page.Replace("-", string.Empty)}";
            var fetchResponse = Client.GetByteArrayAsync(url);
            Task.WaitAll(fetchResponse);
            return Encoding.Default.GetString(fetchResponse.Result);
        }

        static string GetHostService(string host,string url)
        {
            var client = CreateClient(host);
            var fetchResponse = client.GetByteArrayAsync(url);
            Task.WaitAll(fetchResponse);
            return Encoding.Default.GetString(fetchResponse.Result);
        }

        static HttpClient CreateClient(string host)
        {
            return new HttpClient
            {
                Timeout = new TimeSpan(0, 0, 5),
                BaseAddress = new Uri(host)
            };
        }

    }
}