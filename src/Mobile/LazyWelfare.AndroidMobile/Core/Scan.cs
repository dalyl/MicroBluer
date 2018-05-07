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
    public class Scan
    {
        public string Result { get; private set; } = "未识别";

        public Context context { get; private set; }

        public Scan(Activity activity)
        {
            context = activity.ApplicationContext;
        }

        public async Task<bool> Invoke()
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

                var scanner = new MobileBarcodeScanner(context)
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

                var result = await scanner.Scan(opts);
                return ScanResultHandle(result);
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
        private bool ScanResultHandle(ZXing.Result result)
        {
            if (result == null) return false;
            if (string.IsNullOrEmpty(result.Text)) return false;
            Result = result.Text;
            return true;
        }

    }

}