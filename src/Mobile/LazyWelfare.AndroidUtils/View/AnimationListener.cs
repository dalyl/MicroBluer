namespace LazyWelfare.AndroidUtils.View
{
    using System;
    using Android.Views.Animations;
    public class AnimationListener : Java.Lang.Object, Animation.IAnimationListener
    {
        Action<Animation> Start { get; }
        Action<Animation> End { get; }
        Action<Animation> Repeat { get; }

        public AnimationListener(Action<Animation> start = null, Action<Animation> end = null, Action<Animation> repeat = null)
        {
            Start = start;
            End = end;
            Repeat = repeat;
        }

        public void OnAnimationStart(Animation animation)
        {
            Start?.Invoke(animation);
        }


        public void OnAnimationEnd(Animation animation)
        {
            End?.Invoke(animation);
        }


        public void OnAnimationRepeat(Animation animation)
        {
            Repeat?.Invoke(animation);
        }
    }

}