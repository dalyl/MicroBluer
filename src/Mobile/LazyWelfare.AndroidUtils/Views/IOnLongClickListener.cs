namespace LazyWelfare.AndroidUtils.Views
{
    using System;
    using Android.Views;

    public class AnonymousLongClickListener : Java.Lang.Object,View.IOnLongClickListener
    {
        Func<View,bool> LongClick { get; }

        public AnonymousLongClickListener(Func<View, bool> invoke)
        {
            LongClick = invoke;
        }

        public bool OnLongClick(View v)
        {
            var result = LongClick?.Invoke(v);
            return result == null ? false : result.Value;
        }
    }
}