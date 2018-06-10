namespace LazyWelfare.AndroidCtrls.ImageAction
{
    using Android.Animation;
    using Android.Content;
    using Android.Graphics;
    using Android.Graphics.Drawables;
    using Android.Util;
    using Android.Views;
    using Android.Views.Animations;
    using LazyWelfare.AndroidUtils.Animation;
    using System;

    public abstract class ActionImageRenderer
    {
        const long ANIMATION_DURATION = 1333;

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

        public ActionImageView View { get;private set; }

        public ActionImageRenderer(Context context)
        {
            Context = context;

            AnimatorUpdateListener = new AnonymousAnimatorUpdateListener(amor =>
            {
                this.ComputeRender((float)amor.AnimatedValue);
                Callback.InvalidateDrawable(null);
            });

            TypedValue tv = new TypedValue();
            if (context.Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true))
            {
                ActionBarHeight = TypedValue.ComplexToDimensionPixelSize(tv.Data, context.Resources.DisplayMetrics);
            }

            var dm = context.Resources.DisplayMetrics;
            float density = dm.Density;
            Width = dm.WidthPixels;
            Height = dm.HeightPixels - StatusBarHeight - ActionBarHeight;
          

            Duration = ANIMATION_DURATION;

            RenderAnimator = ValueAnimator.OfFloat(0.0f, 1.0f);
            RenderAnimator.RepeatCount = Animation.Infinite;
            RenderAnimator.RepeatMode = ValueAnimatorRepeatMode.Restart;
            RenderAnimator.SetDuration(Duration);
            RenderAnimator.SetInterpolator(new LinearInterpolator());
            RenderAnimator.AddUpdateListener(AnimatorUpdateListener);
        }

        internal int ActionBarHeight { get; }

        int statusBarHeight = 0;

        internal int StatusBarHeight
        {
            get
            {
                if (statusBarHeight == 0) {
                    try
                    {
                        var c = Type.GetType("com.android.internal.R$dimen");
                        var obj = System.Activator.CreateInstance(c);
                        var field = c.GetField("status_bar_height");
                        int x = int.Parse(field.GetValue(obj).ToString());
                        statusBarHeight= Context.Resources.GetDimensionPixelSize(x);
                    }
                    catch (Exception ex)
                    {
                        statusBarHeight = 75;
                    }
                }
                return statusBarHeight;
            }
            set {
                statusBarHeight = value;
            }
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

        public virtual bool TouchArea(MotionEvent act) => false;

    }
}