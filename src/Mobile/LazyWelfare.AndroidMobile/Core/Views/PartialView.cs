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

namespace LazyWelfare.AndroidMobile.Views
{
    public class PartialView
    {
        public const string None = "";
        public const string HomeView = nameof(HomeView);
        public const string HostsView = nameof(HostsView);
        public const string HostDetailView = nameof(HostDetailView);
    }
}