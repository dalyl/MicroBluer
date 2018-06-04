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

        public static (string Host, string Path) Partial = (nameof(PartialView), typeof(HostDetailView).Name);

        public string GenerateStringWithoutModel() => GenerateString();

        public HostModel GetModel(string args)
        {
            return string.IsNullOrEmpty(args) ? new HostModel { Domain = Guid.NewGuid() } : ActiveContext.HostStore.Get(args);
        }
    }

}