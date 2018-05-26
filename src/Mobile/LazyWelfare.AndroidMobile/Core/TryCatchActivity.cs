using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile
{
    public abstract class TryCatchActivity : Activity
    {
        public TryCatch Try { get; }

       // public LoadingView Loading { get; set; }

        public abstract AlphaMaskLayout MaskLayout { get; set; }

        public TryCatchActivity()
        {
            Try = new TryCatchTrust(ShowMessage, ShowLoading, CloseLoading);
        }

        void ShowMessage(string message)
        {
            Toast.MakeText(this, message.Trim(), ToastLength.Short).Show();
        }

        public void ShowLoading()
        {
            //this.RunOnUiThread(()=> Loading.Show());
            MaskLayout.ShowMask();
        }

        public void CloseLoading()
        {
            // this.RunOnUiThread(()=> Loading.Hide());
            MaskLayout.HideMask();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
          //  Loading = new LoadingView(this, Resource.Style.CustomDialog);
        }



    }
}