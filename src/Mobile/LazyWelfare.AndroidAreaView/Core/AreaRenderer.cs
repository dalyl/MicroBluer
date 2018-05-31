namespace LazyWelfare.AndroidAreaView.Core
{
    using Android.Animation;
    using Android.Content;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Views;
    using Android.Views.Animations;
    using LazyWelfare.AndroidUtils.Animation;
    using LazyWelfare.AndroidUtils.Common;
    using System;

    public abstract class AreaRenderer
    {
        const long ANIMATION_DURATION = 1333;
        const float DEFAULT_SIZE = 56.0f;

        public bool IsRunning { get; internal set; }
        public float Height { get; internal set; }
        public float Width { get; internal set; }
        public long Duration { get; internal set; }

        public int Alpha { get; internal set; }
        public ColorFilter ColorFilter { get; internal set; }
        public Rect Bounds { get; internal set; }

        protected virtual void ComputeRender(float renderProgress) { }

        protected abstract void Draw(Canvas canvas, Rect bounds);

        internal void Draw(Canvas canvas) => Draw(canvas, Bounds);

        private ValueAnimator.IAnimatorUpdateListener AnimatorUpdateListener { get; }

        public Drawable.ICallback Callback { private get; set; }

        private ValueAnimator RenderAnimator { get; }

        protected Context Context { get; }

        public AreaRenderer(Context context)
        {
            Context = context;

            AnimatorUpdateListener = new AnonymousAnimatorUpdateListener(amor =>
            {
                this.ComputeRender((float)amor.AnimatedValue);
                Callback.InvalidateDrawable(null);
            });

            Width = DensityUtil.Dip2Px(context, DEFAULT_SIZE);
            Height = DensityUtil.Dip2Px(context, DEFAULT_SIZE);
            Duration = ANIMATION_DURATION;

            RenderAnimator = ValueAnimator.OfFloat(0.0f, 1.0f);
            RenderAnimator.RepeatCount = Android.Views.Animations.Animation.Infinite;
            RenderAnimator.RepeatMode = ValueAnimatorRepeatMode.Restart;
            RenderAnimator.SetDuration(Duration);
            RenderAnimator.SetInterpolator(new LinearInterpolator());
            RenderAnimator.AddUpdateListener(AnimatorUpdateListener);
        }

        internal void Start()
        {
            Reset();
            RenderAnimator.AddUpdateListener(AnimatorUpdateListener);
            RenderAnimator.RepeatCount = ValueAnimator.Infinite;
            RenderAnimator.SetDuration(Duration);
            RenderAnimator.Start();
        }

        internal void Stop()
        {
            RenderAnimator.RemoveUpdateListener(AnimatorUpdateListener);
            RenderAnimator.RepeatCount = 0;
            RenderAnimator.SetDuration(0);
            RenderAnimator.End();
        }

        protected internal virtual void Reset() { }

        public virtual void TouchArea(MotionEvent act) { }

    }
}