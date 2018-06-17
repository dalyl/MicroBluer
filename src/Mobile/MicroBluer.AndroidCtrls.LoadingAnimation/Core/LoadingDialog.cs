namespace MicroBluer.AndroidCtrls.LoadingAnimation
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using MicroBluer.AndroidCtrls.LoadingAnimation.Render;

    public class LoadingDialog : ProgressDialog
    {
        public LoadingDialog(Context context) : base(context)
        {

        }

        public LoadingDialog(Context context, int theme) : base(context, theme)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.Attributes.Width = Width;
            Window.Attributes.Height = Height;

            SetCancelable(Cancelable);
            SetCanceledOnTouchOutside(CanceledOnTouchOutside);

            SetContentView(Resource.Layout.LoadingDialog);

            var view = this.FindViewById<LoadingView>(Resource.Id.WaitingView);
            view.LoadingRenderer = LoadingRendererFactory.CreateLoadingRenderer(Context, Renderer);
          
        }

        /// <summary>
        /// 动画渲染器
        /// </summary>
        public RendererCase Renderer { get; set; } = RendererCase.WhorlLoadingRenderer;

        /// <summary>
        /// Window.Width
        /// </summary>
        public int Width { get; set; } = ViewGroup.LayoutParams.FillParent;

        /// <summary>
        /// Window.Height
        /// </summary>
        public int Height { get; set; } = ViewGroup.LayoutParams.FillParent;

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