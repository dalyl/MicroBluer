namespace MicroBluer.AndroidCtrls.ImageAction
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
    using MicroBluer.AndroidUtils.Graphics;

    public class ActionImageDrawable : Drawable, IAnimatable
    {
        public override int Opacity { get; }

        private ActionImageRenderer AreaRender { get; }

        public ActionImageDrawable( ActionImageRenderer render)
        {
            AreaRender = render;
            AreaRender.Callback = new AnonymousDrawableICallback
            {
                Invalidate = d => InvalidateSelf(),
                Schedule = (d, what, when) => ScheduleSelf(what, when),
                Unschedule = (d, what) => UnscheduleSelf(what),
            };
        }

        public void Start() => this.AreaRender.Start();

        public void Stop() => this.AreaRender.Stop();

        public bool TouchArea(MotionEvent act) => this.AreaRender.TouchArea(act);

        public bool IsRunning => this.AreaRender.IsRunning;

        public override int IntrinsicHeight => (int)this.AreaRender.Height;

        public override int IntrinsicWidth => (int)this.AreaRender.Width;

        public override void SetAlpha(int value) => this.AreaRender.Alpha = value;

        public override void SetColorFilter(ColorFilter value) => this.AreaRender.ColorFilter = value;

        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);
            this.AreaRender.Bounds = bounds;
        }

        public override void Draw(Canvas canvas)
        {
            if (!Bounds.IsEmpty)
            {
                this.AreaRender.Draw(canvas);
            }
        }






    }

}