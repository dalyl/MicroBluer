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
    public abstract class TryCatchActivity : Activity
    {
        public TryCatch Try { get; }

        public abstract AlphaMaskLayout MaskLayout { get; set; }

        public TryCatchActivity()
        {
            Try = new TryCatch(ShowMessage);
        }

        void ShowMessage(string message)
        {
            RunOnUiThread(()=> Toast.MakeText(this, message.Trim(), ToastLength.Short).Show());
        }

    }
}