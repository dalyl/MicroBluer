using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

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

        T DeserializeForm<T>(string args)
        {
            var code = WebUtility.UrlDecode(args);
            var items = code.Split('&');
            var json =new StringBuilder();
            json.Append("{");
            foreach (var it in items)
            {
                var part= it.Replace("=", ":'");
                json.Append($"{part}',");
            }
            json.Append("}");
            var result= json.ToString().Replace(",}","}");
            return JsonConvert.DeserializeObject<T>(result);
        }

    }


   
}