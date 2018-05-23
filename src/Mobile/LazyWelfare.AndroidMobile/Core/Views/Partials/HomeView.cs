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
    public partial class HomeView : IPartialView<HostModel>
    {

        public static (string Host, string Path) Partial = (nameof(WebPartialViews), typeof(HomeView).Name);

        public static string GenerateString(WebPartialActivity context)
        {
            context.RequestStack.Clear();
            context.RequestStack.Push(PartialView.HomeView, "");
            return WebPartialViews.Get(typeof(HomeView).Name, ServiceHost.CurrentHost);
        }

    }
}