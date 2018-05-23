
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

namespace LazyWelfare.AndroidMobile.Script
{
    public class BuinessScript: AndroidScript//注意一定要继承java基类  
    {
        public BuinessScript(Activity activity) : base(activity) { }

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
            service.Delete(args);
            return Try.Show(true, "成功删除");
        }

       
    }


   
}