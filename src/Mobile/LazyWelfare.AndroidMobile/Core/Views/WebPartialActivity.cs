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
using LazyWelfare.AndroidMobile.Script;
using LazyWelfare.AndroidMobile.Views;

namespace LazyWelfare.AndroidMobile
{
    public abstract class WebPartialActivity : TryCatchActivity
    {
        public abstract WebPartialRequestStack RequestStack { get; }

        public LoadingView WaitingView { get;private set; }

        public WebView PartialView { get; set; }

        public WebPartialActivity()
        {
            RequestStack.Clear();
        }

        public bool TryBack()
        {
            var value = RequestStack.Fetch();
            if (value != null)
            {
                PartialView.EvaluateJavascript($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{ nameof(WebPartialViews) }','{ value.Partial.ToString() }','{value.Args}');", null);
                return true;
            }
            return false;
        }

        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
            {
                if (TryBack()) {
                    return true;
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


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public void ShowMaskLayer()
        {
            RunOnUiThread(() => {
                WaitingView = new LoadingView(this);
                WaitingView.Show();
            });
        }

        public void HideMaskLayer()
        {
            RunOnUiThread(() => {
                WaitingView.Dismiss();
            });
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
    }

}