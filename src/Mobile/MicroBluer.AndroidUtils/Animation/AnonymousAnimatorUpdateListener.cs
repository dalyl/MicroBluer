namespace MicroBluer.AndroidUtils.Animation
{
    using Android.Animation;
    using System;

    public class AnonymousAnimatorUpdateListener: Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
    {
        Action<ValueAnimator> Update { get; }
        public AnonymousAnimatorUpdateListener(Action<ValueAnimator> update) { Update = update; }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            Update?.Invoke(animation);
        }
    }
}