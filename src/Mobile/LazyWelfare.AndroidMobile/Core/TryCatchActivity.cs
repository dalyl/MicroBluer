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
    public class TryCatchActivity : Activity
    {
        public TryCatch Try { get; }

        public TryCatchActivity()
        {
            Try = new TryCatch(msg => Toast.MakeText(this, msg.Trim(), ToastLength.Short).Show());
        }
    }
}