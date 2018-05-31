namespace LazyWelfare.AndroidAreaView.Core
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Android.Views;

    using Context = Android.Content.Context;
    using TypedArray = Android.Content.Res.TypedArray;
    using AttributeSet = Android.Util.IAttributeSet;
    using View = Android.Views.View;
    using ImageView = Android.Widget.ImageView;
    using Android.Content.Res;
    using Android.Graphics.Drawables;
    using Android.Graphics;

    public class AreaImageView : ImageView
    {
        private AreaDrawable AreaDrawable { get; set; }

        public AreaImageView(Context context) : base(context) { }

        public AreaImageView(Context context, AttributeSet attrs) : base(context, attrs) { }

        public virtual AreaRenderer AreaRender
        {
            set
            {
                AreaDrawable = new AreaDrawable(value);
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
            AreaDrawable.TouchArea(act);
            return base.OnTouchEvent(act);
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