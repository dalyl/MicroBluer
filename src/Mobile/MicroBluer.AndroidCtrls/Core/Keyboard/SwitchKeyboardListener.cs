namespace MicroBluer.AndroidCtrls.Core.Keyboard
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using Android.InputMethodServices;
    using Java.Lang;
    using MicroBluer.AndroidUtils.InputMethodServices;

    public class KeyboardUtil
    {

        Activity mActivity { get; set; }
        Context mContext { get; set; }
        EditText mEdit { get; set; }

        Keyboard province_keyboard { get; set; }
        Keyboard number_keyboar { get; set; }

        NumberKeyboardView mKeyboardView { get; set; }
        public KeyboardUtil(Activity activity, EditText edit)
        {
            mActivity = activity;
            mContext = (Context)activity;
            mEdit = edit;

            province_keyboard = new Keyboard(mContext, Resource.Xml.province_abbreviation);
            number_keyboar = new Keyboard(mContext, Resource.Xml.number_or_letters);
            mKeyboardView = activity.FindViewById<NumberKeyboardView>(Resource.Id.keyboard_view);
            mKeyboardView.Keyboard = province_keyboard;
            mKeyboardView.Enabled = true;
            mKeyboardView.PreviewEnabled = false;
            mKeyboardView.OnKeyboardActionListener = new KeyboardInputListener
            {
                KeyEvent = (primaryCode, s) =>
                {
                    var editable = mEdit.Text;
                    int start = mEdit.SelectionStart;
                    if (primaryCode ==  (Android.Views.Keycode)(-1))
                    {
                        // 省份简称与数字键盘切换
                        SwitchKeyboard();
                    }
                    else if (primaryCode == (Android.Views.Keycode) (-3))
                    {
                        // 回退
                        if (string.IsNullOrEmpty(editable) ==false)
                        {
                            //没有输入内容时软键盘重置为省份简称软键盘
                            if (editable.Length == 1)
                            {
                                SwitchKeyboard(false);
                            }
                            if (start > 0)
                            {
                                editable.Remove(start - 1, start);
                            }
                        }
                    }
                    else
                    {
                        editable.Insert(start, Character.ToString((char)primaryCode));
                        // 判断第一个字符是否是中文,是，则自动切换到数字软键盘
                        //if (mEdit.Text.matches(reg))
                        //{
                        //    SwitchKeyboard(true);
                        //}
                    }
                },

            };
        }

        /// <summary>
        /// 键盘切换
        /// </summary>
        public void SwitchKeyboard(bool isDefault=true)
        {
        }

        public void HideSoftInputMethod()
        {
            mActivity.Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
            var currentVersion = Android.OS.Build.VERSION.SdkInt;
            string methodName = null;

            if (currentVersion >= BuildVersionCodes.JellyBean)
            {
                // 4.2
                methodName = "setShowSoftInputOnFocus";
            }
            else if (currentVersion >= BuildVersionCodes.IceCreamSandwich)
            {
                // 4.0
                methodName = "setSoftInputShownOnFocus";
            }


            if (methodName == null)
            {
                mEdit.InputType = Android.Text.InputTypes.Null;//(InputType.TYPE_NULL);
            }
            else
            {
                //        Class<EditText> cls = EditText.class;
                //Method setShowSoftInputOnFocus;
                //    try {
                //        setShowSoftInputOnFocus = cls.getMethod(methodName,
                //                boolean.class);
                //        setShowSoftInputOnFocus.setAccessible(true);
                //        setShowSoftInputOnFocus.invoke(mEdit, false);
                //    } catch (NoSuchMethodException e) {
                //        mEdit.setInputType(InputType.TYPE_NULL);
                //        e.printStackTrace();
                //    } catch (IllegalAccessException e) {
                //        e.printStackTrace();
                //    } catch (IllegalArgumentException e) {
                //        e.printStackTrace();
                //    } catch (InvocationTargetException e) {
                //        e.printStackTrace();
                //    }
            }
        }


    }

}