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

    public partial class HostFacultyView : IPartialView<HostModel>
    {
        public const string Placeholder_Append = "#############Append#############";

        public static (string Host, string Path) Partial = (nameof(PartialView), typeof(HostFacultyView).Name);

        public string GenerateStringWithoutModel() => ActiveContext.Try.Show(string.Empty, "获取服务主机信息失败");

        public HostModel GetModel(string args)
        {
            return string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : ActiveContext.Current.HostStore.Get(args);
        }
    }
}