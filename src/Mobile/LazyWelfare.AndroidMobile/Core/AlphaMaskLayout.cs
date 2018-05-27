namespace LazyWelfare.AndroidMobile
{
    using System;
    using System.Collections.Generic;
    using ValueAnimator = Android.Animation.ValueAnimator;
    using Context = Android.Content.Context;
    using TypedArray = Android.Content.Res.TypedArray;
    using Color = Android.Graphics.Color;
    using ColorDrawable = Android.Graphics.Drawables.ColorDrawable;
    using AttributeSet = Android.Util.IAttributeSet;
    using FrameLayout = Android.Widget.FrameLayout;
    using Android.Views;

    /// <summary>
    /// AlphaMaskFrameLayout
    /// Author：Bro0cL on 2016/12/13.
    /// </summary>
    public class AlphaMaskLayout : FrameLayout
    {
        private const int DEFAULT_ALPHA_FROM = 0;
        private const int DEFAULT_ALPHA_TO = 0;
        private const int DEFAULT_DURATION = 0;

        /// <summary>
        ///default foreground color </summary>
        private const string DEFAULT_COLOR = "#1f1f1f";
      
        /// <summary>
        /// start alpha </summary>
        private int alphaFrom;

        /// <summary>
        /// end alpha </summary>
        private int alphaTo;

        /// <summary>
        /// animation duration </summary>
        private int duration;

        /// <summary>
        /// alpha listener </summary>
        private IOnAlphaFinishedListener onAlphaFinishedListener;

        /// <summary>
        /// anim list </summary>
        private List<ValueAnimator> animList = new List<ValueAnimator>();

        public AlphaMaskLayout(Context context) : this(context, null)
        {
        }

        public AlphaMaskLayout(Context context, AttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public AlphaMaskLayout(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            GetAttrs(context, attrs);
        }

        public AlphaMaskLayout(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            GetAttrs(context, attrs);
        }

        private void GetAttrs(Context context, AttributeSet attrs)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.AlphaMaskLayout);
            alphaFrom = a.GetInt(Resource.Styleable.AlphaMaskLayout_aml_alpha_from, DEFAULT_ALPHA_FROM);
            alphaTo = a.GetInt(Resource.Styleable.AlphaMaskLayout_aml_alpha_to, DEFAULT_ALPHA_TO);
            duration = a.GetInt(Resource.Styleable.AlphaMaskLayout_aml_duration, DEFAULT_DURATION);
            a.Recycle();

            //init foreground alpha
            if (Foreground == null)
            {
                Foreground = new ColorDrawable(Color.ParseColor(DEFAULT_COLOR));
            }
            Foreground.Alpha = 0;

            CheckAttrsValues();
        }

        private void CheckAttrsValues()
        {
            if (alphaFrom < 0 || alphaFrom > 255)
            {
                throw new Exception("hey man: the value of alpha_from must be 0~255.");
            }
            if (alphaTo < 0 || alphaTo > 255)
            {
                throw new Exception("hey man: the value of alpha_to must be 0~255.");
            }
            if (duration < 0)
            {
                throw new Exception("hey man: the value of duration must be >0");
            }
        }

        private int ForgroundAlpha
        {
            set
            {
                this.Foreground.Alpha = value;
            }
        }

        private void ExecuteAlphaAnimation(int from, int to, int duration)
        {
            CheckAttrsValues();

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Android.animation.ValueAnimator valueAnimator = Android.animation.ValueAnimator.ofInt(from, to);
            ValueAnimator valueAnimator = ValueAnimator.OfInt(from, to);
            valueAnimator.SetDuration(duration);
            valueAnimator.AddUpdateListener(new AnimatorUpdateListenerAnonymousInnerClass(this, from, to, valueAnimator));
            valueAnimator.Start();
            animList.Add(valueAnimator);
        }

        /// <summary>
        /// get alpha when animation finished or intercepted
        /// @return
        /// </summary>
        private int GetLastFinishedAlpha(bool isShow)
        {
            if (animList.Count == 0)
            {
                return isShow ? alphaFrom : alphaTo;
            }
            else
            {
                ValueAnimator animator = animList[animList.Count - 1];
                animator.Cancel();
                int alpha = (int)animator.AnimatedValue;
                animList.Clear();
                return alpha;
            }
        }

        /// <summary>
        /// set start alpha 0~255 </summary>
        /// <param name="from"> </param>
        public virtual int AlphaFrom
        {
            set
            {
                this.alphaFrom = value;
            }
        }

        /// <summary>
        /// set end alpha 0~255 </summary>
        /// <param name="to"> </param>
        public virtual int AlphaTo
        {
            set
            {
                this.alphaTo = value;
            }
        }

        public virtual int Duration
        {
            set
            {
                this.duration = value;
            }
        }

        public virtual void ShowMask()
        {
            int lastAlpha = GetLastFinishedAlpha(true);
            if (lastAlpha >= alphaTo)
            {
                return;
            }
            ExecuteAlphaAnimation(lastAlpha, alphaTo, duration);
        }

        public virtual void HideMask()
        {
            int lastAlpha = GetLastFinishedAlpha(false);
            if (lastAlpha <= alphaFrom)
            {
                return;
            }
            ExecuteAlphaAnimation(lastAlpha, alphaFrom, duration);
        }

        public void SetOnAlphaFinishedListener(IOnAlphaFinishedListener onAlphaFinishedListener)
        {
            this.onAlphaFinishedListener = onAlphaFinishedListener;
        }

        public interface IOnAlphaFinishedListener
        {
            void OnShowFinished();
            void OnHideFinished();
        }


        private class AnimatorUpdateListenerAnonymousInnerClass : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
        {
            private readonly AlphaMaskLayout outerInstance;

            private int From { get; }

            private int To { get; }

            private ValueAnimator valueAnimator;

            public AnimatorUpdateListenerAnonymousInnerClass(AlphaMaskLayout outerInstance, int from, int to, ValueAnimator valueAnimator)
            {
                this.outerInstance = outerInstance;
                From = from;
                To = to;
                this.valueAnimator = valueAnimator;
            }

            public void OnAnimationUpdate(ValueAnimator animation)
            {
                int newAlpha = (int)animation.AnimatedValue;
                outerInstance.ForgroundAlpha = newAlpha;
                //anim finished
                if (newAlpha == To)
                {
                    valueAnimator.Cancel();
                    outerInstance.animList.Clear();
                    if (outerInstance.onAlphaFinishedListener != null)
                    {
                        if (From > To)
                        {
                            outerInstance.onAlphaFinishedListener.OnHideFinished();
                        }
                        else
                        {
                            outerInstance.onAlphaFinishedListener.OnShowFinished();
                        }
                    }
                }
            }
        }

    }
}