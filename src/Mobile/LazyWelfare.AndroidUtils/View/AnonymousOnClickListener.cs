namespace LazyWelfare.AndroidUtils.View
{
    using System;
    using Android.Views;

    public class AnonymousOnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        Action<View> Click { get; set; }

        public AnonymousOnClickListener(Action<View> click)
        {
            Click = click;
        }

        public void OnClick(View v)
        {
            Click?.Invoke(v);
        }
    }
}
