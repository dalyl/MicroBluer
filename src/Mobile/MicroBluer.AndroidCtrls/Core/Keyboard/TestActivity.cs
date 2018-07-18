namespace MicroBluer.AndroidCtrls.Core.Keyboard
{
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using Android.InputMethodServices;
    using Java.Lang;
    using Android.Views.Animations;
    using Android.Content.PM;

    [Activity(Theme = "@android:style/Theme.NoTitleBar",  ScreenOrientation = ScreenOrientation.Portrait)]
    public class TestActivity : Activity
    {
        public NumberKeyboardView mKeyboardView;
        public View mTargetView;
        public Keyboard mKeyboard;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.KeyboardTest);

            mKeyboard = new Keyboard(this, Resource.Xml.Keyboard_munber);
            mTargetView = (EditText)FindViewById(Resource.Id.target);

            mKeyboardView = FindViewById<NumberKeyboardView>(Resource.Id.keyboard_view);
            mKeyboardView.Keyboard = mKeyboard;

            mTargetView.Touch += (sender, e) =>
            {
                ShowKeyboardWithAnimation();
                e.Handled = true;
            };

            mKeyboardView.Key += (sender, e) =>
            {
                long eventTime = JavaSystem.CurrentTimeMillis();
                KeyEvent ev = new KeyEvent(eventTime, eventTime, KeyEventActions.Down, e.PrimaryCode, 0, 0, 0, 0, KeyEventFlags.SoftKeyboard | KeyEventFlags.KeepTouchMode);
                this.DispatchKeyEvent(ev);
            };
        }

        public void ShowKeyboardWithAnimation()
        {
            if (mKeyboardView.Visibility == ViewStates.Gone)
            {
                Animation animation = AnimationUtils.LoadAnimation(
                    this,
                    Resource.Animation.Sym_keyboard_slide_in_bottom
                );
                mKeyboardView.ShowWithAnimation(animation);
            }
        }
    }
}