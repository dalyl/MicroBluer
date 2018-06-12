using System;
namespace LazyWelfare.AndroidCtrls.LoadingAnimation.Render
{
    using Canvas = Android.Graphics.Canvas;
    using ColorFilter = Android.Graphics.ColorFilter;
    using PixelFormat = Android.Graphics.PixelFormat;
    using Rect = Android.Graphics.Rect;
    using IAnimatable = Android.Graphics.Drawables.IAnimatable;//Animatable;
    using Drawable = Android.Graphics.Drawables.Drawable;
    using Android.Graphics;
    using LazyWelfare.AndroidUtils.Graphics;

    public class LoadingDrawable : Drawable, IAnimatable
    {
        private LoadingRenderer LoadingRender { get; }

        public LoadingDrawable(LoadingRenderer loadingRender)
        {
            LoadingRender = loadingRender;
            LoadingRender.Callback = new AnonymousDrawableICallback
            {
                Invalidate = d => InvalidateSelf(),
                Schedule = (d, what, when) => ScheduleSelf(what, when),
                Unschedule = (d, what) => UnscheduleSelf(what),
            };
        }

        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);
            this.LoadingRender.Bounds = bounds;
        }

        public override void Draw(Canvas canvas)
        {
            if (!Bounds.IsEmpty)
            {
                this.LoadingRender.Draw(canvas);
            }
        }

        public override void SetAlpha(int value) => this.LoadingRender.Alpha = value;

        public override void SetColorFilter(ColorFilter value) => this.LoadingRender.ColorFilter = value;

        public override int Opacity => (int)Format.Translucent;

        public void Start() => this.LoadingRender.Start();

        public void Stop() => this.LoadingRender.Stop();

        public bool IsRunning => this.LoadingRender.Running;

        public override int IntrinsicHeight => (int)this.LoadingRender.Height;

        public override int IntrinsicWidth => (int)this.LoadingRender.Width;
    }

}