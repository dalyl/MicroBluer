using Common.Logging;
using LazyWelfare.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace LazyWelfare.ServerHost.Service
{
    public class ServiceProcess
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(ServiceProcess));

        private ServiceProcess()
        {
            IP = LanService.GetLocalIP();
            IPValue = IP.IpFromString();
        }

        public string IP { get; private set; }

        public IPAddress IPValue { get; private set; }

        public readonly static ServiceProcess Instance = new ServiceProcess();

        void GetSetting(string fileName)
        {
            if (File.Exists(fileName) == false) return;
            var file = new FileInfo(fileName);
            var path = file.Directory.FullName;
            var setting = new ServiceSettings("setting", "CoreWeb.dll", @"E:\Project\Github\LazyWelfare\src\Server\LazyWelfare.ServerWeb\bin\Release\PublishOutput", 6000);
            var value = setting.GetValue();
        }

        void SetSetting()
        {

        }

        #region Process

        public Process Process { get; private set; }

        ProcessStartInfo SetProcessInfo(string args)
        {
            ProcessStartInfo psi;
            psi = new ProcessStartInfo("dotnet");
            psi.Arguments += args;

            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;  //允许重定向标准输出
            psi.RedirectStandardInput = true;   //接受来自调用程序的输入信息

            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            return psi;
        }

        public void Start()
        {
            if (Process != null) return;
            Task.Run(() =>
            {
                var path = @"E:\Project\Github\LazyWelfare\src\Server\CoreWeb\CoreWeb\bin\Release\PublishOutput\";
                var port = FreePort.FindNextAvailableTCPPort(6000);
                var line = $@"{path}CoreWeb.dll --urls=http://*:{port}";
                var psi = SetProcessInfo(line);
                Process = Process.Start(psi);
                Process.OutputDataReceived += new DataReceivedEventHandler(Instance.OnDataReceived);
                Process.BeginOutputReadLine();
                Process.WaitForExit();
                if (Process.ExitCode != 0)
                {
                    logger.Info(Process.StandardError.ReadToEnd());
                }
                Process.Close();
                Process = null;
            });
        }

        public void Stop()
        {
            if (Process == null) return;
            Process.Kill();
            logger.Info("Process Closed");
        }

        void OnDataReceived(object Sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                logger.Info(e.Data);
            }
        }

        #endregion

        #region QRCode


        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);


        public ImageSource CreateQRCode( int width, int height)
        {
            return CreateQRCode("http://baidu.com", width, height);
        }

        ImageSource CreateQRCode(string content, int width, int height)
        {
            EncodingOptions options;
            //包含一些编码、大小等的设置
            //BarcodeWriter :一个智能类来编码一些内容的条形码图像
            BarcodeWriter write = null;
            options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                Margin = 0
            };
            write = new BarcodeWriter();
            //设置条形码格式
            write.Format = BarcodeFormat.QR_CODE;
            //获取或设置选项容器的编码和渲染过程。
            write.Options = options;
            //对指定的内容进行编码，并返回该条码的呈现实例。渲染属性渲染实例使用，必须设置方法调用之前。
            Bitmap bitmap = write.Write(content);
            IntPtr ip = bitmap.GetHbitmap();//从GDI+ Bitmap创建GDI位图对象
                                            //Imaging.CreateBitmapSourceFromHBitmap方法，基于所提供的非托管位图和调色板信息的指针，返回一个托管的BitmapSource
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            return bitmapSource;
        }
        #endregion


    }
}
