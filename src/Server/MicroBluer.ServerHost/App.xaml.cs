using MicroBluer.ServerCore.NamedPipe;
using MicroBluer.ServerHost.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using MicroBluer.ServerCore;
using MicroBluer.ServerHost.Core;

namespace MicroBluer.ServerHost
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        PipeServer PipeServer { get; }

        public App() {
            PipeServer = new PipeServer(new WindowsCommandDistributor());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProcess.Instance.Stop(null);
        }


    }
}
    
