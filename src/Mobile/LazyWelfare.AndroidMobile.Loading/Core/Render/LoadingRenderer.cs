
namespace LazyWelfare.AndroidMobile.Loading.Render
{
    using System;
    using Animator = Android.Animation.Animator;
    using ValueAnimator = Android.Animation.ValueAnimator;
    using Context = Android.Content.Context;
    using Canvas = Android.Graphics.Canvas;
    using ColorFilter = Android.Graphics.ColorFilter;
    using Rect = Android.Graphics.Rect;
    using RectF = Android.Graphics.RectF;
    using Drawable = Android.Graphics.Drawables.Drawable;
    using Handler = Android.OS.Handler;
    using Looper = Android.OS.Looper;
    using Log = Android.Util.Log;
    using Animation = Android.Views.Animations.Animation;
    using LinearInterpolator = Android.Views.Animations.LinearInterpolator;
    using LazyWelfare.AndroidUtils.Common;
    using LazyWelfare.AndroidUtils.Animation;

    public abstract class LoadingRenderer
    {
        private const long ANIMATION_DURATION = 1333;
        private const float DEFAULT_SIZE = 56.0f;

        private ValueAnimator.IAnimatorUpdateListener AnimatorUpdateListener { get; }


        /// <summary>
        /// Whenever <seealso cref="LoadingDrawable"/> boundary changes mBounds will be updated.
        /// More details you can see <seealso cref="LoadingDrawable#onBoundsChange(Rect)"/>
        /// </summary>
        protected internal Rect Bounds { protected get; set; } = new Rect();

        internal Drawable.ICallback Callback { private get; set; }

        private ValueAnimator RenderAnimator;
        protected internal long Duration;
        protected internal float Width;
        protected internal float Height;

        public LoadingRenderer(Context context)
        {
            AnimatorUpdateListener = new AnonymousAnimatorUpdateListener(amor =>
            {
                this.ComputeRender((float)amor.AnimatedValue);
                Callback.InvalidateDrawable(null);
            });
            InitParams(context);
            SetupAnimators();
        }

        protected abstract void Draw(Canvas canvas, Rect bounds);

        internal void Draw(Canvas canvas) => Draw(canvas, Bounds);

        protected internal abstract void ComputeRender(float renderProgress);

        protected internal abstract int Alpha { set; }

        protected internal abstract ColorFilter ColorFilter { set; }

        internal virtual bool Running => RenderAnimator.IsRunning;

        protected internal virtual void AddRenderListener(Animator.IAnimatorListener animatorListener)
        {
            RenderAnimator.AddListener(animatorListener);
        }

        internal virtual void Start()
        {
            Reset();
            RenderAnimator.AddUpdateListener(AnimatorUpdateListener);

            RenderAnimator.RepeatCount = ValueAnimator.Infinite;
            RenderAnimator.SetDuration(Duration);
            RenderAnimator.Start();
        }

        internal virtual void Stop()
        {
            // if I just call mRenderAnimator.end(),
            // it will always call the method onAnimationUpdate(ValueAnimator animation)
            // why ? if you know why please send email to me (dinus_developer@163.com)
            RenderAnimator.RemoveUpdateListener(AnimatorUpdateListener);

            RenderAnimator.RepeatCount = 0;
            RenderAnimator.SetDuration(0);
            RenderAnimator.End();
        }

        protected internal virtual void Reset() { }

        private void InitParams(Context context)
        {
            Width = DensityUtil.Dip2Px(context, DEFAULT_SIZE);
            Height = DensityUtil.Dip2Px(context, DEFAULT_SIZE);
            Duration = ANIMATION_DURATION;
        }

        private void SetupAnimators()
        {
            RenderAnimator = ValueAnimator.OfFloat(0.0f, 1.0f);
            RenderAnimator.RepeatCount = Animation.Infinite;
            RenderAnimator.RepeatMode = Android.Animation.ValueAnimatorRepeatMode.Restart;
            RenderAnimator.SetDuration(Duration);
            //fuck you! the default interpolator is AccelerateDecelerateInterpolator
            RenderAnimator.SetInterpolator(new LinearInterpolator());
            RenderAnimator.AddUpdateListener(AnimatorUpdateListener);
        }

      
    }

}