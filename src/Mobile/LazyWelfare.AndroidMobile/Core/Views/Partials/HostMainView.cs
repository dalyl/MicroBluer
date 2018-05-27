using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Logic;
using LazyWelfare.AndroidMobile.Models;

namespace LazyWelfare.AndroidMobile.Views.Partials
{

    public partial class HostMainView : IPartialView<HostModel>
    {
        public const string Placeholder_Append = "#############Append#############";

        public static (string Host, string Path) Partial = (nameof(WebPartialViews), typeof(HostMainView).Name);
        public static string GenerateString(WebPartialActivity context, string args)
        {
            var service = new HostStoreService(context);
            var model = string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : service.Get(args);
            if (model == null) return context.Try.Show(string.Empty, "获取服务主机信息失败");
            var append = ServiceHost.PageDispatch("command-panel", args, context.Try);
            var page= WebPartialViews.Get(typeof(HostMainView).Name, model);
            return page.Replace(HostMainView.Placeholder_Append, append);
        }
    }
}