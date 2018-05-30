
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
        public BuinessScript(TryCatchActivity activity, WebView brower) : base(activity, brower) { }

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
            return Try.Invoke(false,()=> HostSave(args));
        }

        bool HostSave(string args)
        {
            var model = DeserializeForm<HostModel>(args);
            if (model == null) return Try.Throw<bool>("参数未正确识别");
            var service = new HostStoreService(ViewActivity);
            return Try.Show(() => service.Save(model),"保存成功","保存失败");
        }

        [Export("DeleteHost")]
        [JavascriptInterface]
        public bool DeleteHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确识别");
            var service = new HostStoreService(ViewActivity);
            return Try.Show(()=>service.Delete(args), "成功删除","删除失败");
        }

        [Export("SetHost")]
        [JavascriptInterface]
        public bool SetHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确识别");
            var userService = new UserStoreService(ViewActivity);
            var result= Try.Show(() => userService.SetAttr("Host", args), "设置成功", "设置失败");
            if (result) {
                var hostService = new HostStoreService(ViewActivity);
                var host = Try.Invoke(null, () => hostService.Get(args));
                ServiceHost.SetHost(host, Try);
            }
            return result;
        }

        [Export("CommandSumbit")]
        [JavascriptInterface]
        public bool CommandSumbit(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Throw<bool>("参数未正确识别");

            var ctrl = new ImageCtrlView(ViewActivity);
            ctrl.Show();

            return Try.Show<bool>(true, args);
        }
    }



}