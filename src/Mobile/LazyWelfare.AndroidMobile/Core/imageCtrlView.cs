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
    public  class ImageCtrlView : ProgressDialog
    {
        public ImageCtrlView(Context context) : base(context)
        {
        }

        public ImageCtrlView(Context context, int theme) : base(context, theme)
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.Attributes.Width = Width;
            Window.Attributes.Height = Height;

            SetCancelable(Cancelable);
            SetCanceledOnTouchOutside(CanceledOnTouchOutside);

            SetContentView(Resource.Layout.ImageCtrl);

            var view = this.FindViewById<ImageView>(Resource.Id.imageCtrl);
            view.SetImageResource(Resource.Drawable.offered_vol);

            var exitView = this.FindViewById<LinearLayout>(Resource.Id.imageCtrl_Exit);
           // exitView.SetOnClickListener


            var exitBtn = this.FindViewById<Button>(Resource.Id.imageCtrl_Exit_btn);

        }
       
        /// <summary>
        /// Window.Width
        /// </summary>
        public int Width { get; set; } = ViewGroup.LayoutParams.WrapContent;

        /// <summary>
        /// Window.Height
        /// </summary>
        public int Height { get; set; } = ViewGroup.LayoutParams.WrapContent;

        /// <summary>
        /// 是否可以取消
        /// </summary>
        public bool Cancelable { get; set; } = false;

        /// <summary>
        /// 是否可以触摸取消
        /// </summary>
        public bool CanceledOnTouchOutside { get; set; } = false;

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