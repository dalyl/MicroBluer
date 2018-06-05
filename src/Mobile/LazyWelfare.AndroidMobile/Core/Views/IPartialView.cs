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
    public interface IPartialView
    {
        void PushRequest(PartialActivity context, string args);

        string GenerateString(string args);
    }

    public interface IPartialView<T> : IPartialView
    {
        T Model { get; set; }
    }
}