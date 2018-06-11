namespace LazyWelfare.AndroidUtils.Views
{
    using Android.Views;
    using System;

    public class AnonymousOnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        /// <summary>
        /// 防止短时间多次点击
        /// </summary>
        private static long lastTimeMillis;
        private static long MIN_CLICK_INTERVAL = 1000;

        Action<View> Click { get; set; }

        public AnonymousOnClickListener(Action<View> click)
        {
            Click = click;
        }

        public void OnClick(View v)
        {
            long currentTimeMillis = DateTime.Now.Ticks;
            if ((currentTimeMillis - lastTimeMillis) > MIN_CLICK_INTERVAL)
            {
                lastTimeMillis = currentTimeMillis;
                Click?.Invoke(v);
            }
            return;
        }
    }
}
