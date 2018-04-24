using Common.Logging;
using LazyWelfare.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp
{

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private static readonly ILog _logger = Common.Logging.LogManager.GetLogger<App>();

        public App() {

            //  CheckAdministrator();
        }

        /// <summary>  
        /// 检查是否是管理员身份  
        /// </summary>  
        private void CheckAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,  
                // so instead we launch the app as administrator in a new process.  
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator  
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process  
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                // Shut down the current process  
                Environment.Exit(0);
            }
        }
      
    }
}
