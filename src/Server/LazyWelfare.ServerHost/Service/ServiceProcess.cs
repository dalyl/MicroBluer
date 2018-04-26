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

        public Process Process { get; private set; }

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


    }
}
