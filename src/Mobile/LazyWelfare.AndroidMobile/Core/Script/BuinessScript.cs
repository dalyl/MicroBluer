﻿namespace LazyWelfare.AndroidMobile.Script
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
                var model= ActiveContext.Current.Host.GetServiceDefine(scan.Result);
                if (model != null) {

                    ActiveContext.Current.HostStore.Save(model);
                }
            }
        }

        [Export("SaveHost")]
        [JavascriptInterface]
        public bool SaveHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false,"参数未正确提供");
            return Try.Invoke(false,()=> HostSave(args));
        }

        bool HostSave(string args)
        {
            var model = Try.Invoke(null, () => DeserializeForm<HostModel>(args));
            if (model == null) return Try.Show<bool>(false, "参数未正确识别");
            return Try.Show(() => ActiveContext.Current.HostStore.Save(model),"保存成功","保存失败");
        }

        [Export("DeleteHost")]
        [JavascriptInterface]
        public bool DeleteHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            return Try.Show(()=> ActiveContext.Current.HostStore.Delete(args), "成功删除","删除失败");
        }

        [Export("SetHost")]
        [JavascriptInterface]
        public bool SetHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var result= Try.Show(() => ActiveContext.Current.UserStore.SetAttr("Host", args), "设置成功", "设置失败");
            if (result) ActiveContext.CurrentHost = Try.Invoke(null, () => ActiveContext.Current.HostStore.Get(args));
            return result;
        }

        [Export("CommandSumbit")]
        [JavascriptInterface]
        public bool CommandSumbit(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var model = Try.Invoke(null, () => JsonConvert.DeserializeObject<Argument>(args));
            if (model == null) return Try.Show<bool>(false, "参数未正确识别");
            return ActiveContext.Current.Host.InvokeCommand(model, ViewActivity);
        }



    }



}