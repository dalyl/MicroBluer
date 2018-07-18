namespace MicroBluer.AndroidCtrls.Core.Keyboard
{
    using System;
    using Android.Content;
    using Android.InputMethodServices;
    using Android.Util;
    using Android.Views;
    using Android.Views.Animations;

    public class NumberKeyboardView : KeyboardView
    {
        public NumberKeyboardView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.Press += new EventHandler<PressEventArgs>(OnPress);
        }


        void OnPress(object sender, PressEventArgs e)
        {

        }

        public void ShowWithAnimation(Animation animation)
        {
            animation.AnimationEnd += (sender, e) => {
                Console.WriteLine("Set visibility!");
                Visibility = ViewStates.Visible;
            };

            Animation = animation;
            Visibility = ViewStates.Invisible;
        }


    }
}