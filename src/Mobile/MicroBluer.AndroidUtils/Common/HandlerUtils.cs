namespace MicroBluer.AndroidUtils.Common
{
    using Handler = Android.OS.Handler;
    using Looper = Android.OS.Looper;
    using System;

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