namespace LazyWelfare.AndroidCtrls.ImageAction
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using LazyWelfare.AndroidCtrls.ImageAction.Renderer;
    using LazyWelfare.AndroidUtils.Views;

    public  class ImageActionDialog : AlertDialog
    {
        public ImageActionDialog(Context context) : base(context)
        {
        }

        public ImageActionDialog(Context context, int theme) : base(context, theme)
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

            var view = this.FindViewById<ActionImageView>(Resource.Id.imageCtrl);
            view.AreaRender = FetchRender?.Invoke(Context);

            var top = this.FindViewById<RelativeLayout>(Resource.Id.top_layout); 
            var exitView = top.FindViewById<LinearLayout>(Resource.Id.imageCtrl_Exit);
            exitView.SetOnClickListener(new AnonymousOnClickListener(v => Dismiss()));
        }

        /// <summary>
        /// 设置渲染器
        /// </summary>
        public Func<Context, ActionImageRenderer> FetchRender { get; set; } = context => new VolumeControllerRenderer(context) ;

        public ActionImageRenderer Render
        {
            set
            {
                FetchRender = context => value;
            }
        }

        /// <summary>
        /// Window.Width
        /// </summary>
        public int Width { get; set; } = ViewGroup.LayoutParams.MatchParent;

        /// <summary>
        /// Window.Height
        /// </summary>
        public int Height { get; set; } = ViewGroup.LayoutParams.MatchParent;

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