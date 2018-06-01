namespace LazyWelfare.AndroidMobile.Script
{

    using Android.Webkit;
    using Java.Interop;
    using LazyWelfare.AndroidMobile.Logic;
    using LazyWelfare.AndroidMobile.Models;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class BuinessScript: AndroidScript//注意一定要继承java基类  
    {
        public BuinessScript(ActiveActivity activity, WebView brower) : base(activity, brower)
        {
        }

        [Export("ScanHost")]
        [JavascriptInterface]
        public void ScanHost()
        {
            var scan = new ScanPlugin(ViewActivity);
            var invoke = scan.Invoke();
            Task.WaitAll(invoke);
            if (invoke.Result == false)
            {
                ShowToast("已取消");
            }
            if (string.IsNullOrEmpty(scan.Result))
            {
                ShowToast("未识别");
            }
            else
            {
                var model= ServiceHost.GetServiceDefine(scan.Result);
                if (model != null) {
                    var service = new HostStoreService();
                    service.Save(model);
                }
            }
        }

        [Export("SaveHost")]
        [JavascriptInterface]
        public bool SaveHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确提供");
            return Try.Invoke(false,()=> HostSave(args));
        }

        bool HostSave(string args)
        {
            var model = Try.Invoke(null, () => DeserializeForm<HostModel>(args));
            if (model == null) return Try.Throw<bool>("参数未正确识别");
            var service = new HostStoreService();
            return Try.Show(() => service.Save(model),"保存成功","保存失败");
        }

        [Export("DeleteHost")]
        [JavascriptInterface]
        public bool DeleteHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确提供");
            var service = new HostStoreService();
            return Try.Show(()=>service.Delete(args), "成功删除","删除失败");
        }

        [Export("SetHost")]
        [JavascriptInterface]
        public bool SetHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确提供");
            var userService = new UserStoreService();
            var result= Try.Show(() => userService.SetAttr("Host", args), "设置成功", "设置失败");
            if (result) {
                var hostService = new HostStoreService();
                var host = Try.Invoke(null, () => hostService.Get(args));
                ServiceHost.SetHost(host);
            }
            return result;
        }

        [Export("CommandSumbit")]
        [JavascriptInterface]
        public bool CommandSumbit(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确提供");
            var model = Try.Invoke(null, () => JsonConvert.DeserializeObject<Argument>(args));
            if (model == null) return Try.Throw<bool>("参数未正确识别");
            return ServiceHost.InvokeCommand(model, ViewActivity);
        }



    }



}