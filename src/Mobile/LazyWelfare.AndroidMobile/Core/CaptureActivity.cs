using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZXing;
using ZXing.Mobile;

namespace LazyWelfare.AndroidMobile
{

    [Activity]
    public class CaptureActivity  : Activity
    {
        public string Result { get; private set; } = "未识别";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Scan);
            var succeeded = await Invoke();
            Appcation.ScanResultDo?.Invoke(succeeded, Result);
            Appcation.ScanResultDo = null;
            Finish();
        }

        async Task<bool> Invoke()
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
                    // CustomOverlay=,

                    FlashButtonText = "识别",
                    CancelButtonText = "取消",

                    //设置上下提示文字
                    TopText = "请将条形码对准方框内",
                    BottomText = "确认后按下右下角识别按钮"
                };


                View zxingOverlay;
                //使用自定义界面（可以给框内加个动画什么的，这个自由发挥）
                scanner.UseCustomOverlay = true;
                zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.ZxingOverlay, null);
                scanner.CustomOverlay = zxingOverlay;

                //var zxingOverlay = LayoutInflater.FromContext(this).Inflate(Resource.Layout.Code, null);
                //var doneButton = zxingOverlay.FindViewById<Button>(Resource.Id.buttonZxingDone);
                //doneButton.Click += (sender, e) =>
                //{
                //    scanner.Cancel();
                //    SetResult(Result.Canceled);
                //    Finish();
                //};
                //scanner.CustomOverlay = zxingOverlay;

                var invoke = await scanner.Scan(opts);
                return ScanResultHandle(invoke);
            }
            catch (Exception ex)
            {
                Result = ex.Message;
                return false;
            }
        }

   
        /// <summary>
        /// 获取扫描结果的处理
        /// </summary>
        bool ScanResultHandle(ZXing.Result result)
        {
            if (result == null) return false;
            if (string.IsNullOrEmpty(result.Text)) return false;
            Result = result.Text;
            return true;
        }

        void HandleScanResult(ZXing.Result result)
        {
            if (result == null || string.IsNullOrEmpty(result.Text))
            {
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "扫描已取消！", ToastLength.Short).Show();
                });
                return;
            }
            else
            {
                //扫描成功 偶尔扫描结果会是一串数字???
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, result.Text, ToastLength.Short).Show();
                });
            }
        }

    }

}