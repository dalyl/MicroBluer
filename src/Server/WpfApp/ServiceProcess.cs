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
 
namespace WpfApp
{
    public class ServiceProcess
    {
        private static readonly ILog logger = LogManager.GetLogger(nameof(ServiceProcess));

        private ServiceProcess()
        {
            IP = LanService.GetLocalIP();
            IPValue= IP.IpFromString();
        }

        public string IP { get; private set; }

        public IPAddress IPValue { get; private set; }

        public readonly static ServiceProcess Instance = new ServiceProcess();

        void GetSetting(string fileName)
        {
            if (File.Exists(fileName) == false) return;
            var file = new FileInfo(fileName);
            var path = file.Directory.FullName;
            var setting = new ServiceSettings("setting","CoreWeb.dll", @"E:\Project\Github\LazyWelfare\src\Server\CoreWeb\CoreWeb\bin\Release\PublishOutput", 16000);
            var value = setting.GetValue();
        }

        public void ExecuteTool(string toolFile, string args)
        {
            Process p;
            ProcessStartInfo psi;
            psi = new ProcessStartInfo(toolFile);
            //psi.Arguments += args;

            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;  //允许重定向标准输出
            psi.RedirectStandardInput = true;   //接受来自调用程序的输入信息

            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            p = Process.Start(psi);
            p.OutputDataReceived += new DataReceivedEventHandler(OnDataReceived);
            p.BeginOutputReadLine();
            p.StandardInput.WriteLine(args);

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                logger.Info(p.StandardError.ReadToEnd());
            }
            p.Close();
        }


        void OnDataReceived(object Sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                logger.Info(e.Data);
            }
        }


    }
}
