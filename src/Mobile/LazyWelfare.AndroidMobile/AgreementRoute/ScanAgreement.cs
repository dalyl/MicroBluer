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
using ZXing;
using ZXing.Mobile;

namespace LazyWelfare.AndroidMobile
{
    public class ScanAgreement : IAgreementHandle
    {
        public bool Result { get; private set; } = false;

        public Context Activity { get; private set; }

        public WebView WebBrower { get; private set; }

        public string RequestContent { get;private set; }

        public void Init(WebView webView, string url)
        {
            WebBrower = webView;
            Activity = WebBrower.Context;
            RequestContent = url;
            InitScan();
          //  webView.Context.StartActivity(typeof(ScanActivity));
            Result = true;
        }


        private async void InitScan()
        {
            try
            {
                var opts = new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.QR_CODE
                    }
                };

                opts.CharacterSet = "";

                var scanner = new MobileBarcodeScanner(WebBrower.Context)
                {
                    //不使用自定义界面
                    UseCustomOverlay = false,

                    FlashButtonText = "识别",
                    CancelButtonText = "取消",

                    //设置上下提示文字
                    TopText = "请将条形码对准方框内",
                    BottomText = "确认后按下右下角识别按钮"
                };

                var result = await scanner.Scan(opts);
                var handle = ScanResultHandle(result);
                if (!handle) ScanCancel();
            }
            catch (Exception ex)
            {
                ScanFail();
            }
        }


        /// <summary>
        /// 获取扫描结果的处理
        /// </summary>
        private bool ScanResultHandle(ZXing.Result result)
        {
            if (result == null) return false;
            if (string.IsNullOrEmpty(result.Text)) return false;
            string url = result.Text;
            //若扫描结果包含https:// 或者 http:// 则跳转网页
            if (url.Contains("https://") || url.Contains("http://"))
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse(result.Text);
                Intent intent = new Intent(Intent.ActionView, uri);
                Activity.StartActivity(intent);
            }
            var _barcodeFormat = "扫描结果" + result.Text;
            Toast.MakeText(WebBrower.Context, _barcodeFormat, ToastLength.Short).Show();
            return true;
        }

        void ScanCancel()
        {
            Toast.MakeText(Activity, "扫描取消", ToastLength.Short).Show();
           // Activity.StartActivity(typeof(MainActivity));
        }

        void ScanFail()
        {
            Toast.MakeText(Activity, "扫描失败", ToastLength.Short).Show();
          //  Activity.StartActivity(typeof(MainActivity));
        }
    }
}