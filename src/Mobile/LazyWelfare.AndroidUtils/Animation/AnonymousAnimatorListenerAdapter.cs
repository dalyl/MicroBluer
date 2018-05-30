
namespace LazyWelfare.AndroidUtils.Animation
{
    using System;
    using Android.Animation;

    public class AnonymousAnimatorListenerAdapter : AnimatorListenerAdapter
    {

        public Action<Animator> Cancel { get; set; }
        public override void OnAnimationCancel(Animator animator)
        {
            base.OnAnimationCancel(animator);
            Cancel?.Invoke(animator);
        }

        public Action<Animator> End { get; set; }
        public override void OnAnimationEnd(Animator animator)
        {
            base.OnAnimationEnd(animator);
            End?.Invoke(animator);
        }

        public Action<Animator> Pause { get; set; }
        public override void OnAnimationPause(Animator animator)
        {
            base.OnAnimationPause(animator);
            Pause?.Invoke(animator);
        }

        public Action<Animator> Repeat { get; set; }
        public override void OnAnimationRepeat(Animator animator)
        {
            base.OnAnimationRepeat(animator);
            Repeat?.Invoke(animator);
        }

        public Action<Animator> Resume { get; set; }
        public override void OnAnimationResume(Animator animator)
        {
            base.OnAnimationResume(animator);
            Resume?.Invoke(animator);

        }

        public Action<Animator> Start { get; set; }
        public override void OnAnimationStart(Animator animator)
        {
            base.OnAnimationStart(animator);
            Start?.Invoke(animator);

        }

    }
}