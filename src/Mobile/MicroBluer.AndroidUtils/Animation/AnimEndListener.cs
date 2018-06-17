namespace MicroBluer.AndroidUtils.Animation
{
    using Android.Animation;
    using System;

    public class AnimEndListener : AnimatorListenerAdapter
    {
        Action<Animator> End { get; }

        public AnimEndListener(Action<Animator> end)
        {
            End = end;
        }

        public override void OnAnimationEnd(Animator animation)
        {
            base.OnAnimationEnd(animation);
            End?.Invoke(animation);
        }
    }
}