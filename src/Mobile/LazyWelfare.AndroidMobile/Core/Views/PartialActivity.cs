namespace LazyWelfare.AndroidMobile
{
    using System;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Webkit;
    using Android.Widget;
    using LazyWelfare.AndroidCtrls.LoadingAnimation;
    using LazyWelfare.AndroidMobile.Views;

    public abstract class PartialActivity : ActiveActivity
    {
        public abstract PartialRequestStack RequestStack { get; }

        public LoadingDialog WaitingView { get;private set; }

        public WebView PartialView { get; set; }

        public PartialActivity()
        {
            RequestStack.Clear();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public bool TryBack()
        {
            var value = RequestStack.Fetch();
            if (value != null)
            {
                PartialView.EvaluateJavascript($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{ nameof(Views.PartialHost) }','{ value.Partial.ToString() }','{value.Args}');", null);
                return true;
            }
            return false;
        }

        #region --- OnKeyDown ---

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

        #endregion

        #region --- WaitingView ---

        public void ShowMaskLayer()
        {
            RunOnUiThread(() => {
                WaitingView = CreateLoadingDialog();
                WaitingView.Show();
            });
        }

        LoadingDialog CreateLoadingDialog()
        {
            var loading = new LoadingDialog(this)
            {
                Width = ViewGroup.LayoutParams.WrapContent,
                Height = ViewGroup.LayoutParams.WrapContent,

            };
          
            return loading;
        }

        public void HideMaskLayer()
        {
            RunOnUiThread(() => {
                WaitingView.Dismiss();
            });
        }

        #endregion

    }

}