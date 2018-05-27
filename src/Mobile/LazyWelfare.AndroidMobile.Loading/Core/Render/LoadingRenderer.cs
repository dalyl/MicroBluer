
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

    public abstract class LoadingRenderer
    {
        private const long ANIMATION_DURATION = 1333;
        private const float DEFAULT_SIZE = 56.0f;

        private ValueAnimator.IAnimatorUpdateListener AnimatorUpdateListener { get; }

        private class AnimatorUpdateListenerAnonymousInnerClass : Java.Lang.Object,ValueAnimator.IAnimatorUpdateListener
        {
            LoadingRenderer outerInstance { get; }
            public AnimatorUpdateListenerAnonymousInnerClass(LoadingRenderer master)
            {
                outerInstance = master;
            }

            public  void OnAnimationUpdate(ValueAnimator animation)
            {
                outerInstance.ComputeRender((float)animation.AnimatedValue);
                outerInstance.InvalidateSelf();
            }
        }

        /// <summary>
        /// Whenever <seealso cref="LoadingDrawable"/> boundary changes mBounds will be updated.
        /// More details you can see <seealso cref="LoadingDrawable#onBoundsChange(Rect)"/>
        /// </summary>
        protected internal readonly Rect mBounds = new Rect();

        private Drawable.ICallback mCallback;
        private ValueAnimator mRenderAnimator;

        protected internal long mDuration;

        protected internal float mWidth;
        protected internal float mHeight;

        public LoadingRenderer(Context context)
        {
            AnimatorUpdateListener = new AnimatorUpdateListenerAnonymousInnerClass(this);
            InitParams(context);
            SetupAnimators();
        }

        [Obsolete]
        protected internal virtual void Draw(Canvas canvas, Rect bounds)
        {
        }

        protected internal virtual void Draw(Canvas canvas) => Draw(canvas, mBounds);

        protected internal abstract void ComputeRender(float renderProgress);

        protected internal abstract int Alpha { set; }

        protected internal abstract ColorFilter ColorFilter { set; }

        protected internal abstract void Reset();

        protected internal virtual void AddRenderListener(Animator.IAnimatorListener animatorListener)
        {
            mRenderAnimator.AddListener(animatorListener);
        }

        internal virtual void Start()
        {
            Reset();
            mRenderAnimator.AddUpdateListener(AnimatorUpdateListener);

            mRenderAnimator.RepeatCount = ValueAnimator.Infinite;
            mRenderAnimator.SetDuration(mDuration);
            mRenderAnimator.Start();
        }

        internal virtual void Stop()
        {
            // if I just call mRenderAnimator.end(),
            // it will always call the method onAnimationUpdate(ValueAnimator animation)
            // why ? if you know why please send email to me (dinus_developer@163.com)
            mRenderAnimator.RemoveUpdateListener(AnimatorUpdateListener);

            mRenderAnimator.RepeatCount = 0;
            mRenderAnimator.SetDuration(0) ;
            mRenderAnimator.End();
        }

        internal virtual bool Running
        {
            get
            {
                return mRenderAnimator.IsRunning;
            }
        }

        internal virtual Drawable.ICallback Callback
        {
            set
            {
                this.mCallback = value;
            }
        }

        internal virtual Rect Bounds
        {
            set
            {
                mBounds.Set(value);
            }
        }

        private void InitParams(Context context)
        {
            mWidth = DensityUtil.dip2px(context, DEFAULT_SIZE);
            mHeight = DensityUtil.dip2px(context, DEFAULT_SIZE);

            mDuration = ANIMATION_DURATION;
        }

        private void SetupAnimators()
        {
            mRenderAnimator = ValueAnimator.OfFloat(0.0f, 1.0f);
            mRenderAnimator.RepeatCount = Animation.Infinite;
            mRenderAnimator.RepeatMode = Android.Animation.ValueAnimatorRepeatMode.Restart;
            mRenderAnimator.SetDuration(mDuration);
            //fuck you! the default interpolator is AccelerateDecelerateInterpolator
            mRenderAnimator.SetInterpolator(new LinearInterpolator());
            mRenderAnimator.AddUpdateListener(AnimatorUpdateListener);
        }

        private void InvalidateSelf()
        {
            mCallback.InvalidateDrawable(null);
        }

    }

}