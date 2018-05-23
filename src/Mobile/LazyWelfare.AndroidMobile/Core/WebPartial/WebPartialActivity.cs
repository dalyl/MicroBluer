using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using LazyWelfare.AndroidMobile.Views;

namespace LazyWelfare.AndroidMobile
{
    public abstract class WebPartialActivity : Activity
    {
        public abstract WebPartialRequestStack RequestStack { get; }

        protected WebView PartialView { get; set; }

        public WebPartialActivity()
        {
            RequestStack.Clear();
        }

        //DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        //public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        //{
        //    if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
        //    {
        //        if (!lastBackKeyDownTime.HasValue || DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 2))
        //        {
        //            Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
        //            lastBackKeyDownTime = DateTime.Now;
        //        }
        //        else
        //        {
        //            Finish();
        //        }
        //        return true;
        //    }
        //    return base.OnKeyDown(keyCode, e);
        //}

        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
            {
                if (RequestStack.IsEmpty == false)
                {
                    var value = RequestStack.Fetch();
                    PartialView.EvaluateJavascript($"ViewScript.PartialLoad('#MainContent','{ nameof(WebViews) }','{ value.Partial.ToString() }','{value.Args}');", null);
                }
                else if (!lastBackKeyDownTime.HasValue || DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 4))
                {
                    Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
                    lastBackKeyDownTime = DateTime.Now;
                }
                else
                {
                    Finish();
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }

}