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
using ZXing;
using ZXing.Mobile;

namespace LazyWelfare.Mobile.Android
{
    public class ScanActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Scan);

            //初始化扫描仪,不然会报错空引用
            //MobileBarcodeScanner.Initialize(Application);

            InitScan();
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

                var scanner = new MobileBarcodeScanner(this)
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
                if (!string.IsNullOrEmpty(result.Text))
                {
                    ScanResultHandle(result);
                    //若扫描结果包含https:// 或者 http:// 则跳转网页
                    if (result.Text.Contains("https://") || result.Text.Contains("http://"))
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(result.Text);
                        Intent intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                        Finish();
                    }
                }

                var _barcodeFormat = result?.BarcodeFormat.ToString() ?? string.Empty;
                var _barcodeData = result?.Text ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 获取扫描结果的处理
        /// </summary>
        private void ScanResultHandle(ZXing.Result result)
        {
            string url = result.Text;
            if (!string.IsNullOrEmpty(url))
            {
                var _barcodeFormat = "扫描结果" + result.Text;
            }
            else
            {
                var _barcodeFormat = "扫描取消";
            }
        }
    }
}