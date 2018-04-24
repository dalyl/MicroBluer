using Common.Logging;
using LazyWelfare.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp.Pages
{
    /// <summary>
    /// StatePage.xaml 的交互逻辑
    /// </summary>
    public partial class StatePage : UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger<StatePage>();

        public StatePage()
        {
            InitializeComponent();
        }

        private void Button_ServiceStart_Click(object sender, RoutedEventArgs e)
        {
            var port = FreePort.FindNextAvailableTCPPort(60000);
            //E:\Projects\GitHub\LazyWelfare\src\Server\CoreWeb\CoreWeb\bin\Release\PublishOutput
            var path = @"E:\Project\Github\LazyWelfare\src\Server\CoreWeb\CoreWeb\bin\Release\PublishOutput\";
            var line = $@"dotnet  {path}CoreWeb.dll --urls=""http://*:{port}""";
            //CommandTask cmd = new CommandTask(path);
            //cmd.Exec(line);

            ExecuteTool("cmd.exe", line);

        }


        private  void ExecuteTool(string toolFile, string args)
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

        private  void OnDataReceived(object Sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                logger.Info(e.Data);
            }
        }

        private void Button_ServiceStop_Click(object sender, RoutedEventArgs e)
        {


        }
    }
}
