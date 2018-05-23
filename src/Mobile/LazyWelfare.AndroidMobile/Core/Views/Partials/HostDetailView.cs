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
    public partial class HostDetailView : IPartialView<HostModel>
    {
        public static (string Host, string Path) Partial = (nameof(WebPartialViews), typeof(HostDetailView).Name);
        public static string GenerateString(WebPartialActivity context, string args)
        {
            context.RequestStack.Push(PartialView.HostsView, args);
            var service = new HostStoreService(context);
            var model = string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : service.Get(args);
            return WebPartialViews.Get(typeof(HostDetailView).Name, model);
        }
    }
}