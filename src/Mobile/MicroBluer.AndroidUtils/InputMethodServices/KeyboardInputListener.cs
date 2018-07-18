

namespace MicroBluer.AndroidUtils.InputMethodServices
{
    using System;
    using Android.InputMethodServices;
    using Java.Lang;

    public class KeyboardInputListener : Java.Lang.Object, KeyboardView.IOnKeyboardActionListener
    {
        public Action<Android.Views.Keycode, Android.Views.Keycode[]> KeyEvent { get; set; }

        public Action<Android.Views.Keycode> PressEvent { get; set; }

        public Action<Android.Views.Keycode> ReleaseEvent { get; set; }

        public Action<ICharSequence> TextEvent { get; set; }

        public Action SwipeDownEvent { get; set; }

        public Action SwipeLeftEvent { get; set; }

        public Action SwipeRightEvent { get; set; }

        public Action SwipeUpEvent { get; set; }


        public void OnKey(Android.Views.Keycode primaryCode, Android.Views.Keycode[] keyCodes)
        {
            KeyEvent?.Invoke(primaryCode, keyCodes);
        }

        public void OnPress(Android.Views.Keycode primaryCode)
        {
            PressEvent?.Invoke(primaryCode);
        }

        public void OnRelease(Android.Views.Keycode primaryCode)
        {
            ReleaseEvent?.Invoke(primaryCode);
        }

        public void OnText(ICharSequence text)
        {
            TextEvent?.Invoke(text);
        }

        public void SwipeDown()
        {
            SwipeDownEvent?.Invoke();
        }

        public void SwipeLeft()
        {
            SwipeLeftEvent?.Invoke();
        }

        public void SwipeRight()
        {
            SwipeUpEvent?.Invoke();
        }

        public void SwipeUp()
        {
            SwipeRightEvent?.Invoke();
        }
    }

}