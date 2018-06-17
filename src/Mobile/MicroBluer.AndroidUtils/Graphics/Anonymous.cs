

namespace MicroBluer.AndroidUtils.Graphics
{
    using Drawable = Android.Graphics.Drawables.Drawable;
    using Object=Java.Lang.Object;
    using Java.Lang;
    using System;

    public class AnonymousDrawableICallback: Object,Drawable.ICallback
    {
        public Action<Drawable> Invalidate { get; set; }

        public void InvalidateDrawable(Drawable d)
        {
            Invalidate?.Invoke(d);
        }

        public Action<Drawable, IRunnable, long> Schedule { get; set; }
        public void ScheduleDrawable(Drawable d, IRunnable what, long when)
        {
            Schedule?.Invoke(d, what,when);
        }

        public Action<Drawable, IRunnable> Unschedule { get; set; }
        public void UnscheduleDrawable(Drawable d, IRunnable what)
        {
            Unschedule?.Invoke(d, what);
        }

    }
}