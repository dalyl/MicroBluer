namespace LazyWelfare.AndroidMobile.Script
{

    using Android.Webkit;
    using Java.Interop;
    using LazyWelfare.AndroidCtrls.FolderSelect;
    using LazyWelfare.AndroidMobile.Models;
    using LazyWelfare.AndroidMobile.Views;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public class BuinessScript: AndroidScript //注意一定要继承java基类  
    {
        public BuinessScript(ActiveActivity activity, WebView brower) : base(activity, brower)
        {
        }

        #region --- Host ---


        [Export("ScanHost")]
        [JavascriptInterface]
        public void ScanHost()
        {
            var scan = new ScanPlugin(ViewActivity,"添加主机");
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
                var model= ActiveContext.HostExpress.GetServiceDefine(scan.Result);
                if (model != null) {

                    ActiveContext.HostStore.Save(model);
                }
            }
        }

        [Export("SaveHost")]
        [JavascriptInterface]
        public bool SaveHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false,"参数未正确提供");
            var model = Try.Invoke(null, () => DeserializeForm<HostModel>(args));
            if (model == null) return Try.Show<bool>(false, "参数未正确识别");
            return Try.Show(() => ActiveContext.HostStore.Save(model), "保存成功", "保存失败");
        }

        [Export("DeleteHost")]
        [JavascriptInterface]
        public bool DeleteHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            return Try.Show(() => ActiveContext.HostStore.Delete(args), "成功删除", "删除失败");
        }

        [Export("SetHost")]
        [JavascriptInterface]
        public bool SetHost(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var result= Try.Show(() => ActiveContext.UserStore.SetHost(args), "设置成功", "设置失败");
            return result;
        }

        #endregion

        #region --- FolderMap   ---

        [Export("SaveFolderMap")]
        [JavascriptInterface]
        public bool SaveFolderMap(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var model = Try.Invoke(null, () => DeserializeForm<FolderMapModel>(args));
            if (model == null) return Try.Show<bool>(false, "参数未正确识别");
            return Try.Show(() => ActiveContext.FolderMapStore.Save(model), "保存成功", "保存失败");
        }

        [Export("DeleteFolderMap")]
        [JavascriptInterface]
        public bool DeleteFolderMap(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            return Try.Show(() => ActiveContext.FolderMapStore.Delete(args), "成功删除", "删除失败");
        }

        [Export("GetSrcFolder")]
        [JavascriptInterface]
        public string GetSrcFolder(string path)
        {
            var fetchResult = FolderSelector.SelectSingle(ViewActivity, SelectorType.Directory, path);
            Task.WaitAll(fetchResult);
            return fetchResult.Result;
        }

        [Export("PullMapFiles")]
        [JavascriptInterface]
        public bool PullMapFiles(string args)
        {
            void job() => ActiveContext.FolderMapStore.PullMapFiles(args);
            var worker = new PartialBackgroudWorkerAsyncTask(ViewActivity as PartialActivity, job);
            worker.Execute();
            return true;
        }

        [Export("PushMapFiles")]
        [JavascriptInterface]
        public bool PushMapFiles(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            void job() => ActiveContext.FolderMapStore.PushMapFiles(args);
            var worker = new PartialBackgroudWorkerAsyncTask(ViewActivity as PartialActivity, job);
            worker.Execute();
            return true;
        }


        #endregion

        [Export("CommandSumbit")]
        [JavascriptInterface]
        public bool CommandSumbit(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var model = Try.Invoke(null, () => JsonConvert.DeserializeObject<Argument>(args));
            if (model == null) return Try.Show<bool>(false, "参数未正确识别");
            return ActiveContext.HostExpress.InvokeCommand(model, ViewActivity);
        }

    }

}