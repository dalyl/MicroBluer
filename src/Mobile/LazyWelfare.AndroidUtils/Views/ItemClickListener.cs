namespace LazyWelfare.AndroidUtils.Views
{
    using System;
    using Android.Views;
    using Android.Widget;

    public class ItemClickListener : Java.Lang.Object,AdapterView.IOnItemClickListener
    {
        Action<AdapterView,View, int> _invoke { get;  }

        public ItemClickListener(Action<AdapterView, View, int> invoke)
        {
            _invoke = invoke;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _invoke?.Invoke(parent, view, position);
        }
    }
}