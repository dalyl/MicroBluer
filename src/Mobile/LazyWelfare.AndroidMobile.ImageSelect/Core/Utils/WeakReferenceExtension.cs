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
    public static class WeakReferenceExtension
    {
        public static T Get<T>(this WeakReference<T> model_ref) where T:class
        {
            T model = default(T);
            model_ref.TryGetTarget(out model);
            return model;
        }

    }

    
}