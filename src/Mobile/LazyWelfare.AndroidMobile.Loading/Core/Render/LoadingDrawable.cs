using System;
namespace LazyWelfare.AndroidMobile.Loading.Render
{
    using Canvas = Android.Graphics.Canvas;
    using ColorFilter = Android.Graphics.ColorFilter;
    using PixelFormat = Android.Graphics.PixelFormat;
    using Rect = Android.Graphics.Rect;
    using IAnimatable = Android.Graphics.Drawables.IAnimatable;//Animatable;
    using Drawable = Android.Graphics.Drawables.Drawable;
    using Android.Graphics;
    using Java.Lang;
    using LazyWelfare.AndroidUtils.Graphics;

    public class LoadingDrawable : Drawable, IAnimatable
    {
        private readonly LoadingRenderer mLoadingRender;

        private ICallback mCallback { get; }

        public LoadingDrawable(LoadingRenderer loadingRender)
        {
            mCallback = new AnonymousDrawableICallback
            {
                Invalidate = d => InvalidateSelf(),
                Schedule = (d, what, when) => ScheduleSelf(what, when),
                Unschedule = (d, what) => UnscheduleSelf(what),
            };
            mLoadingRender = loadingRender;
            mLoadingRender.Callback = mCallback;
        }

        protected  override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);
            this.mLoadingRender.Bounds = bounds;
        }

        public override void Draw(Canvas canvas)
        {
            if (!Bounds.IsEmpty)
            {
                this.mLoadingRender.Draw(canvas);
            }
        }

        public override void SetAlpha(int value)
        {
            this.mLoadingRender.Alpha = value;
        }

        public override void SetColorFilter(ColorFilter value)
        {
            this.mLoadingRender.ColorFilter = value;
        }

        public override int Opacity => (int)Format.Translucent;

        public  void Start()
        {
            this.mLoadingRender.Start();
        }

        public  void Stop()
        {
            this.mLoadingRender.Stop();
        }

        public  bool IsRunning
        {
            get
            {
                return this.mLoadingRender.Running;
            }
        }

        public override int IntrinsicHeight
        {
            get
            {
                return (int)this.mLoadingRender.mHeight;
            }
        }

        public override int IntrinsicWidth
        {
            get
            {
                return (int)this.mLoadingRender.mWidth;
            }
        }
    }

}