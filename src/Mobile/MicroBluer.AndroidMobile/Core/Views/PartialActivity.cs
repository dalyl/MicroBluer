namespace MicroBluer.AndroidMobile
{
    using System;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Webkit;
    using Android.Widget;
    using MicroBluer.AndroidCtrls.LoadingAnimation;
    using MicroBluer.AndroidMobile.Views;
    using MicroBluer.AndroidMobile.WebAgreement;

    public abstract class PartialActivity : ActiveActivity
    {

        public LoadingDialog WaitingView { get;private set; }

        public WebView PartialView { get; set; }

        public PartialActivity(PartialRequestStack requestStack)
        {
            RequestStack = requestStack;
            RequestStack.Clear();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public bool TryBack()
        {
            var value = RequestStack.Fetch();
            if (value != AgreementUri.Empty)
            {
                return OpenWebview(value);
            }
            return false;
        }

        public bool OpenWebview(AgreementUri uri, string args)
        {
            return OpenWebview(string.IsNullOrEmpty(args) ? uri : new AgreementUri(uri, args));
        }

        public bool OpenWebview(AgreementUri uri)
        {
            PartialView.EvaluateJavascript($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{uri.Host}','{uri.Path}',{(string.IsNullOrEmpty(uri.Args) ? "null": $"'{uri.Args}'")});", null);
            return true;
        }

        #region ---   PartialRequestStack   ---

        private PartialRequestStack RequestStack { get; }

        public void StackPush(AgreementUri uri, string args = "")
        {
            RequestStack.Push(string.IsNullOrEmpty(args) ? uri : new AgreementUri(uri, args));
        }

        public void StackClear()
        {
            RequestStack.Clear();
        }

        public void StackClearPush(AgreementUri uri, string args = "")
        {
            RequestStack.Clear();
            RequestStack.Push(string.IsNullOrEmpty(args) ? uri : new AgreementUri(uri, args));
        }


        #endregion

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