namespace MicroBluer.AndroidMobile.Script
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Android.Webkit;
    using Java.Interop;
    using MicroBluer.AndroidCtrls;
    using MicroBluer.AndroidCtrls.FileSelect;
    using MicroBluer.AndroidMobile.Models;
    using MicroBluer.AndroidMobile.Views;
    using MicroBluer.AndroidUtils;
    using MicroBluer.AndroidUtils.IO;
    using MicroBluer.AndroidMobile.WebAgreement;
    using MicroBluer.AndroidMobile.Views.Partials;

    public class BuinessScript: AndroidScript //注意一定要继承java基类  
    {
        public BuinessScript(ActiveActivity activity, WebView brower) : base(activity, brower)
        {
        }

        #region  --- User ---

        [Export("SaveUser")]
        [JavascriptInterface]
        public bool SaveUser(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var model = Try.Invoke(null, () => DeserializeForm<UserModel>(args));
            if (model == null) return Try.Show<bool>(false, "参数未正确识别");
            return Try.Show(() => ActiveContext.UserStore.Save(model), "保存成功", "保存失败");
        }

        #endregion

        #region --- Host ---


        [Export("ScanHost")]
        [JavascriptInterface]
        public void ScanHost()
        {
            var scan = new CodeScaner(ViewActivity,"添加主机");
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

        [Export("FileExpleror")]
        [JavascriptInterface]
        public bool FileExpleror()
        {
            TryCatch.Current.Invoke(() => AndroidCtrls.FileExpler.FileExpleror.OpenDialog(ViewActivity));
            return true;
        }

        [Export("FilePrivateExpleror")]
        [JavascriptInterface]
        public bool FilePrivateExpleror()
        {
            if (string.IsNullOrEmpty(ActiveContext.User.Folder)) return Try.Show<bool>(false, "用户文件箱尚未设置");
            TryCatch.Current.Invoke(() => AndroidCtrls.FileExpler.FileExpleror.OpenDialog(ViewActivity, new List<string> { ActiveContext.User.Root }));
            return true;
        }

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
            var fetchResult = FileSelector.SelectSingle(ViewActivity, SelectorType.Directory, path);
            Task.WaitAll(fetchResult);
            return fetchResult.Result;
        }

        [Export("CollectMapFiles")]
        [JavascriptInterface]
        public bool CollectMapFiles(string args)
        {
            if(string.IsNullOrEmpty(ActiveContext.User.Folder)) return Try.Show<bool>(false, "用户文件箱尚未设置");
            void job() => ActiveContext.FolderMapStore.CollectMapFiles(args);
            var worker = new PartialBackgroudWorkerAsyncTask(ViewActivity as PartialActivity, job);
            worker.Execute();
            return true;
        }

        [Export("RevertMapFiles")]
        [JavascriptInterface]
        public bool RevertMapFiles(string args)
        {
            if(string.IsNullOrEmpty(ActiveContext.User.Folder)) return Try.Show<bool>(false, "用户文件箱尚未设置");
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            void job() => ActiveContext.FolderMapStore.RevertMapFiles(args);
            var worker = new PartialBackgroudWorkerAsyncTask(ViewActivity as PartialActivity, job);
            worker.Execute();
            return true;
        }

        [Export("ScanFileMaps")]
        [JavascriptInterface]
        public bool ScanFileMaps(string args)
        {
            if (string.IsNullOrEmpty(ActiveContext.User.Folder)) return Try.Show<bool>(false, "用户文件箱尚未设置");
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            List<string> dirs =null;
            void job() => dirs = ActiveContext.FolderMapStore.ScanFileMaps(args);
            bool after()
            {
                if(dirs==null) return Try.Show<bool>(false, "扫描返回值异常");
                var view = ViewActivity as PartialActivity;
                var json = JsonConvert.SerializeObject(dirs);
                view.OpenWebview(FolderMapExplerorView.Partial, json);
                return true;
            }
            var worker = new PartialBackgroudWorkerAsyncTask(ViewActivity as PartialActivity, job, null, () => after());
            worker.Execute();
            return true;
        }


        #endregion

        #region --- HostsFile ---

        [Export("SaveHostsFile")]
        [JavascriptInterface]
        public bool SaveHostsFile(string args)
        {
            if (string.IsNullOrEmpty(args)) return Try.Show<bool>(false, "参数未正确提供");
            var path = @"/system/etc/hosts";
            var apkRoot = "chmod 777 " + path;
            var IsRoot= SystemManager.RootCommand(apkRoot);
            if(IsRoot==false) return Try.Show<bool>(false, "尚未获得 root 权限不能操作hosts 文件");
            TryCatch.Current.Invoke(()=>File.WriteAllText(path, args));
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