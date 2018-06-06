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
using LazyWelfare.AndroidMobile.ImageSelect;
using LazyWelfare.AndroidMobile.ImageSelect.Engine;
using ZXing;
using ZXing.Common;
using ZXing.Mobile;
using ZXing.QrCode;

namespace LazyWelfare.AndroidMobile
{
    using Bitmap= Android.Graphics.Bitmap;
    using BitmapFactory = Android.Graphics.BitmapFactory;

    public class ScanPlugin
    {
        public string Result { get; private set; } = "未识别";

        public Activity Context { get; }

        public string Title { get;}

        public ScanPlugin(Activity activity,string title)
        {
            Context = activity;
            Title = title;
            Init();
        }

        private void Init()
        {
            var zxingOverlay = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.ZxingOverlay, null);

            var bulbBtn = zxingOverlay.FindViewById(Resource.Id.scanBulbBtn);
            var albumBtn = zxingOverlay.FindViewById(Resource.Id.scanAlbumBtn);

            bulbBtn.Click += Bulb_Click;
            albumBtn.Click += Album_Click;

            var bar= zxingOverlay.FindViewById(Resource.Id.top_layout);
            var title=bar.FindViewById<TextView>(Resource.Id.context_Title);
            var back=bar.FindViewById(Resource.Id.context_btback);

            title.Text = Title;
            back.Click += Back_Click;

            MobileBarcodeScanner.Initialize(Context.Application);
            var scanner = new MobileBarcodeScanner()
            {
                //使用自定义界面
                UseCustomOverlay = true,
                CustomOverlay = zxingOverlay,
            };

            OverScan = scanner.Cancel;
            ShowScan = async (opts) => await scanner.Scan(opts);
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
                var result = await ShowScan(opts);
                return ScanResultHandle(result);
            }
            catch (Exception ex)
            {
                Result = ex.Message;
                return false;
            }
        }

        Func<MobileBarcodeScanningOptions, Task<ZXing.Result>> ShowScan { get; set; }

        Action OverScan { get; set; }

        Android.Net.Uri PickerResult { get; set; }

        #region ---  Album_Click  ---

        public static int REQUEST_CODE_CHOOSE = 1;

        public void Album_Click(object sender, EventArgs e)
        {
            try
            {
                // SelectImageByImgStore();
                BulbPlugin.From(Context)
                    .SingleChoice()
                    .EnableCamera(false)
                    .SetEngine(new GlideEngine())
                    .ForResult(urls =>
                    {
                        if (urls != null && urls.Count > 0)
                        {
                            PickerResult = urls.FirstOrDefault();
                            OverScan();
                        }
                    });
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, "App Select Photo Gallery Error:" + ex.InnerException, ToastLength.Short).Show();
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
            Context.StartActivityForResult(_intentCut, 1);
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
                Toast.MakeText(Context, "App Open Photo FlashLight Error:" + ex.InnerException, ToastLength.Short).Show();
            }
        }

        bool lightStatus = false;
        private void lightSwitch()
        {
            try
            {
                var manager = (CameraManager)Context.GetSystemService(Android.Content.Context.CameraService);
                lightStatus = !lightStatus;
                manager.SetTorchMode("0", lightStatus);
            }
            catch
            {
                throw (new Exception("未发现照明灯"));
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
                var pm = Context.PackageManager;
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

        #region ---  Back_Click  ---

        public void Back_Click(object sender, EventArgs e)
        {
            OverScan();
        }

        #endregion

        /// <summary>
        /// 获取扫描结果的处理
        /// </summary>
        private bool ScanResultHandle(ZXing.Result result)
        {
            if (PickerResult != null) return PickerResultHandle();
            if (result == null) return false;
            Result = result.Text;
            return true;
        }

        private bool PickerResultHandle()
        {
            var path = GetImagePath(PickerResult);
            var map= CreateBitmap(path);
            Result = YuvHandle(map);
            return true;
        }

        Bitmap CreateBitmap(string path)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            //options.InJustDecodeBounds = true; // 先获取原大小

            //return BitmapFactory.DecodeFile(path, options);

            options.InJustDecodeBounds = false; // 获取新的大小
            int sampleSize = (int)(options.OutHeight / (float)200);
            if (sampleSize <= 0)
                sampleSize = 1;
            options.InSampleSize = sampleSize;

            return BitmapFactory.DecodeFile(path, options);
        }

        string GetImagePath(Android.Net.Uri uri)
        {
            String[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
            var actualimagecursor = Context.ContentResolver.Query(uri, proj, null, null, null);
            int actual_image_column_index = actualimagecursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
            actualimagecursor.MoveToFirst();
            return actualimagecursor.GetString(actual_image_column_index);
        }

        //string RgbHandle(Bitmap scanBitmap)
        //{
          
        //    RGBLuminanceSource source = new RGBLuminanceSource(scanBitmap, scanBitmap.Width, scanBitmap.Height);
        //    BinaryBitmap bitmap1 = new BinaryBitmap(new HybridBinarizer(source));
        //    QRCodeReader reader = new QRCodeReader();

        //    try
        //    {
        //        // DecodeHintType 和EncodeHintType
        //        var hints = new Dictionary<DecodeHintType, object>();
        //        hints.Add(DecodeHintType.CHARACTER_SET, "utf-8"); // 设置二维码内容的编码
        //        var result= reader.decode(bitmap1, hints);
        //        return result.Text;
        //    }
        //    catch
        //    {
        //        return string.Empty;
        //    }
        //}

        string YuvHandle(Bitmap scanBitmap)
        {
            LuminanceSource source1 = new PlanarYUVLuminanceSource(rgb2YUV(scanBitmap), scanBitmap.Width, scanBitmap.Height, 0, 0, scanBitmap.Width, scanBitmap.Height, false);
            BinaryBitmap binaryBitmap = new BinaryBitmap(new HybridBinarizer(source1));
            MultiFormatReader reader1 = new MultiFormatReader();
            try
            {
                var result = reader1.decode(binaryBitmap);
                return result.Text;
            }
            catch 
            {
                return string.Empty;
            }
        }

        byte[] rgb2YUV(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[] pixels = new int[width * height];
            bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);

            int len = width * height;
            byte[] yuv = new byte[len * 3 / 2];
            int y, u, v;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int rgb = pixels[i * width + j] & 0x00FFFFFF;

                    int r = rgb & 0xFF;
                    int g = (rgb >> 8) & 0xFF;
                    int b = (rgb >> 16) & 0xFF;

                    y = ((66 * r + 129 * g + 25 * b + 128) >> 8) + 16;
                    u = ((-38 * r - 74 * g + 112 * b + 128) >> 8) + 128;
                    v = ((112 * r - 94 * g - 18 * b + 128) >> 8) + 128;

                    y = y < 16 ? 16 : (y > 255 ? 255 : y);
                    u = u < 0 ? 0 : (u > 255 ? 255 : u);
                    v = v < 0 ? 0 : (v > 255 ? 255 : v);

                    yuv[i * width + j] = (byte)y;
                    //                yuv[len + (i >> 1) * width + (j & ~1) + 0] = (byte) u;
                    //                yuv[len + (i >> 1) * width + (j & ~1) + 1] = (byte) v;
                }
            }
            return yuv;
        }


    }
}