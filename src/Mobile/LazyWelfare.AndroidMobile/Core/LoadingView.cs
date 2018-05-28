using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile
{

    public class LoadingView : ProgressDialog
    {
        public LoadingView(Context context) : base(context)
        {
        }

        public LoadingView(Context context, int theme) : base(context, theme)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetCancelable(true);
            SetCanceledOnTouchOutside(false);

            SetContentView(Resource.Layout.Loading);
            Window.Attributes.Width = ViewGroup.LayoutParams.WrapContent;
            Window.Attributes.Height = ViewGroup.LayoutParams.WrapContent;
        }
       

        public override void Show()
        {
            //开启
            base.Show();
        }

        public override void Dismiss()
        {
            //关闭
            base.Dismiss();
        }
    }
}