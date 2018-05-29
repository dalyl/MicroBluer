using LazyWelfare.ServerCore.NamedPipe;
using LazyWelfare.ServerHost.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using LazyWelfare.ServerCore;
using LazyWelfare.ServerHost.Core;

namespace LazyWelfare.ServerHost
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
    
