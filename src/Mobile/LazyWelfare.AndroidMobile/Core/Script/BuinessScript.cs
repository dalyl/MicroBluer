
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
using System.Threading.Tasks;

namespace LazyWelfare.AndroidMobile.Script
{
    public class BuinessScript: AndroidScript//注意一定要继承java基类  
    {
        public BuinessScript(TryCatchActivity activity) : base(activity) { }

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
                var model= ServiceHost.GetServiceDefine(scan.Result,Try);
                if (model != null) {
                    var service = new HostStoreService(ViewActivity);
                    service.Save(model);
                }
            }
        }

        [Export("SaveHost")]
        [JavascriptInterface]
        public bool SaveHost(string args)
        {
            return Try.Invoke(false,()=> saveHost(args));
        }

        bool saveHost(string args)
        {
            var model = DeserializeForm<HostModel>(args);
            if (model == null) return Try.Throw<bool>("参数未正确识别");
            var service = new HostStoreService(ViewActivity);
            service.Save(model);
            return Try.Show(true,"保存成功");
        }

        [Export("DeleteHost")]
        [JavascriptInterface]
        public bool DeleteHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确识别");

            var service = new HostStoreService(ViewActivity);
            var result= service.Delete(args);
            return Try.Show(result, "成功删除","删除失败");
        }

        [Export("SetHost")]
        [JavascriptInterface]
        public bool SetHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确识别");

            var userService = new UserStoreService(ViewActivity);
            var result = userService.SetAttr("Host", args);
            var set= Try.Show(result, "设置成功", "设置失败");
            if (set) {
                var hostService = new HostStoreService(ViewActivity);
                var host = hostService.Get(args);
                ServiceHost.SetHost(host, Try);
            }
            return set;
        }

        [Export("CommandSumbit")]
        [JavascriptInterface]
        public bool CommandSumbit(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确识别");


            return Try.Throw<bool>(args);
        }
    }



}