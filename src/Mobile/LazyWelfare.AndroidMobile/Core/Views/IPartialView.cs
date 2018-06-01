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
        string GenerateString();

    }

    public interface IPartialView<T> : IPartialView
    {
        T Model { get; set; }

        T GetModel(string args);

        string GenerateStringWithoutModel();
    }
}