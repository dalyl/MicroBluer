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
    public partial class HostsView :  IPartialView<List<HostModel>>
    {
        public static (string Host, string Path) Partial = (nameof(PartialView), typeof(HostsView).Name);

        public string GenerateStringWithoutModel() => GenerateString();

        public List<HostModel> GetModel(string args)
        {
            return ActiveContext.Current.HostStore.GetList();
        }
    }
}