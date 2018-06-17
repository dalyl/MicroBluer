namespace MicroBluer.AndroidCtrls.LoadingAnimation
{
    using System;
    using Android.Views;

    using Context = Android.Content.Context;
    using TypedArray = Android.Content.Res.TypedArray;
    using AttributeSet = Android.Util.IAttributeSet;
    using View = Android.Views.View;
    using ImageView = Android.Widget.ImageView;

    using LoadingDrawable =Render.LoadingDrawable;
    using LoadingRenderer = Render.LoadingRenderer;
    using LoadingRendererFactory = Render.LoadingRendererFactory;

    public class LoadingView : ImageView
    {
        private LoadingDrawable mLoadingDrawable;

        public LoadingView(Context context) : base(context)
        {
        }

        public LoadingView(Context context, AttributeSet attrs) : base(context, attrs)
        {
            InitAttrs(context, attrs);
        }

        private void InitAttrs(Context context, AttributeSet attrs)
        {
            try
            {
                TypedArray ta = context.ObtainStyledAttributes(attrs, Resource.Styleable.LoadingView);
                int loadingRendererId = ta.GetInt(Resource.Styleable.LoadingView_loading_renderer, 0);
                LoadingRenderer = LoadingRendererFactory.CreateLoadingRenderer(context, loadingRendererId);
                ta.Recycle();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }

        public virtual LoadingRenderer LoadingRenderer
        {
            set
            {
                mLoadingDrawable = new LoadingDrawable(value);
                SetImageDrawable (mLoadingDrawable);
            }
        }

        protected  override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            StartAnimation();
        }

        protected  override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            StopAnimation();
        }

        protected  override void OnVisibilityChanged(View changedView, ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);

            bool visible = visibility ==  ViewStates.Visible && Visibility == ViewStates.Visible;
            if (visible)
            {
                StartAnimation();
            }
            else
            {
                StopAnimation();
            }
        }

        private void StartAnimation()
        {
            if (mLoadingDrawable != null)
            {
                mLoadingDrawable.Start();
            }
        }

        private void StopAnimation()
        {
            if (mLoadingDrawable != null)
            {
                mLoadingDrawable.Stop();
            }
        }
    }
}

