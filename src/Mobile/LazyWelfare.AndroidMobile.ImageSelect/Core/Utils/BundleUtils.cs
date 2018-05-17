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

namespace LazyWelfare.AndroidMobile.ImageSelect.Utils
{
    public class BundleUtils
    {
        public static String BuildKey<T>(string name)
        {
            return $"{typeof(T).Namespace}.{typeof(T).Name}.{name}";
        }
    }
}