using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Provider;
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
                var bulbBtn = zxingOverlay.FindViewById(Resource.Id.scanBulbBtn);
                var albumBtn = zxingOverlay.FindViewById(Resource.Id.scanAlbumBtn);

                bulbBtn.Click += Bulb_Click;
                albumBtn.Click += Album_Click;

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


        #region ---  Album_Click  ---

        public void Album_Click(object sender, EventArgs e)
        {
            try
            {
                // SelectImageByImgStore();
                Intent intent = new Intent(context, typeof(ImageSelectActivity));
                context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "App Select Photo Gallery Error:" + ex.InnerException, ToastLength.Short).Show();
            }

        }

        /// <summary>
        /// 调用相册选择
        /// </summary>
        private void SelectImageByImgStore()
        {
            Intent _intentCut = new Intent(Intent.ActionGetContent, null);
            _intentCut.SetType("image/*");// 设置文件类型
            var sdcardTempFile = new Java.IO.File("/mnt/sdcard/", "tmp_pic_" + SystemClock.CurrentThreadTimeMillis() + ".jpg");
            _intentCut.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(sdcardTempFile));
            _intentCut.PutExtra(MediaStore.ExtraVideoQuality, 1);
            context.StartActivityForResult(_intentCut, 1);
        }

        #endregion

        #region ---  Bulb_Click  ---

       // 

        public void Bulb_Click(object sender, EventArgs e)
        {
            try
            {
                //if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
                //{
                //    lightSwitch();
                //}
                //else {
                //    lightSwitchM();
                //}

                lightSwitchM();
 
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, "App Open Photo FlashLight Error:" + ex.InnerException, ToastLength.Short).Show();
            }
        }

        bool lightStatus = false;
        private void lightSwitch()
        {
            try
            {
                var manager = (CameraManager)context.GetSystemService(Context.CameraService);
                lightStatus = !lightStatus;
                manager.SetTorchMode("0", lightStatus);
            }
            catch {
                throw (new  Exception("未发现照明灯"));
            }
        }

        Camera m_Camera { get; set; } = null;
        private void lightSwitchM()
        {
            if (m_Camera != null)
            {
                // 关闭手电筒
                m_Camera.StopPreview();
                m_Camera.Release();
                m_Camera = null;
            }
            else
            {
                // 打开手电筒
                var pm = context.PackageManager;
                var features = pm.GetSystemAvailableFeatures();
                foreach (var f in features)
                {
                    if (PackageManager.FeatureCameraFlash.Equals(f.Name))
                    {
                        // 判断设备是否支持闪光灯
                        if (null == m_Camera)
                        {
                            m_Camera = Camera.Open();
                        }
                        var parameters = m_Camera.GetParameters();
                        parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                        m_Camera.SetParameters(parameters);
                        m_Camera.StartPreview();
                    }
                }
            }
        }

        void Dispose()
        {
            if (m_Camera != null)
            {
                m_Camera.StopPreview();
                m_Camera.Release();
                m_Camera = null;
            }
        }


        #endregion


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