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

    public class LoadingDrawable : Drawable, IAnimatable
    {
        private readonly LoadingRenderer mLoadingRender;

        private ICallback mCallback { get; }

        private class CallbackAnonymousInnerClass : Java.Lang.Object,ICallback
        {
            LoadingDrawable Master { get; }
            public CallbackAnonymousInnerClass(LoadingDrawable master)
            {
                Master = master;
            }

            public  void InvalidateDrawable(Drawable d)
            {
                Master.InvalidateSelf();
            }

            public  void ScheduleDrawable(Drawable d, IRunnable what, long when)
            {
                Master.ScheduleSelf(what, when);
            }

            public  void UnscheduleDrawable(Drawable d, IRunnable what)
            {
                Master.UnscheduleSelf(what);
            }
        }

        public LoadingDrawable(LoadingRenderer loadingRender)
        {
            mCallback = new CallbackAnonymousInnerClass(this);
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