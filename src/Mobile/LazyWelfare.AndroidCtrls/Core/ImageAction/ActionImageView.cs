namespace LazyWelfare.AndroidCtrls.ImageAction
{
    using Android.Views;
    using Context = Android.Content.Context;
    using AttributeSet = Android.Util.IAttributeSet;
    using View = Android.Views.View;
    using ImageView = Android.Widget.ImageView;

    public class ActionImageView : ImageView
    {
        private ActionImageDrawable AreaDrawable { get; set; }

        public ActionImageView(Context context) : base(context) { }

        public ActionImageView(Context context, AttributeSet attrs) : base(context, attrs) { }

        public virtual ActionImageRenderer AreaRender
        {
            set
            {
                AreaDrawable = new ActionImageDrawable(value);
                SetImageDrawable(AreaDrawable);
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            StartAnimation();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            StopAnimation();
        }

        public override bool OnTouchEvent(MotionEvent act)
        {
            return AreaDrawable.TouchArea(act);
           // return base.OnTouchEvent(act);
        }

        protected override void OnVisibilityChanged(View changedView, ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);

            bool visible = visibility == ViewStates.Visible && Visibility == ViewStates.Visible;
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
            if (AreaDrawable != null)
            {
                AreaDrawable.Start();
            }
        }

        private void StopAnimation()
        {
            if (AreaDrawable != null)
            {
                AreaDrawable.Stop();
            }
        }

    }

}