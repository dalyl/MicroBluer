using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Models;

namespace LazyWelfare.AndroidMobile.Views.Partials
{
    public interface IsView
    {
        string GenerateString();

    }

    public interface IsView<T>: IsView
    {
        T Model { get; set; }
    }

    public partial class HostsView : IsView<List<HostModel>> { }

    public partial class HomeView : IsView { }

}