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

namespace LazyWelfare.AndroidMobile
{
    public sealed class StoreService
    {
        private StoreService() { }

        public static readonly StoreService Provider = new StoreService();

        public SharedPreferences Shared(string file) => new SharedPreferences(file);

    }
}