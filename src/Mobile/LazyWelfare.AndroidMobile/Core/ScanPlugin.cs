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
    public class ScanPlugin
    {
        public string Result { get; private set; } = "未识别";

        public Activity context { get; private set; }

        public ScanPlugin(Activity activity)
        {
            context = activity;
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
                    },
                    CharacterSet = ""
                };

                View zxingOverlay = LayoutInflater.FromContext(context).Inflate(Resource.Layout.ZxingOverlay, null);

                var scanner = new MobileBarcodeScanner(context)
                {
                    //使用自定义界面
                    UseCustomOverlay = true,
                    CustomOverlay = zxingOverlay,
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