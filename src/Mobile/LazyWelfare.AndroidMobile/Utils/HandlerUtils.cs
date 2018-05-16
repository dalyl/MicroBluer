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

namespace LazyWelfare.AndroidMobile.Utils
{
    using Handler = Android.OS.Handler;
    using Looper = Android.OS.Looper;

    public class HandlerUtils
    {
        private HandlerUtils()
        {
        }

        public static Handler MainHandler
        {
            get
            {
                return new Handler(Looper.MainLooper);
            }
        }

        public static void PostOnMain(Action message)
        {
            MainHandler.Post(message);
        }

        public static void PostOnMainWithDelay(Action message, long delayMillis)
        {
            MainHandler.PostDelayed(message, delayMillis);
        }
    }
}