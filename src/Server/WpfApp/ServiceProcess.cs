using LazyWelfare.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
 
namespace WpfApp
{
    public class ServiceProcess
    {
        private ServiceProcess()
        {
            IP = LanService.GetLocalIP();
            IPValue= IP.IpFromString();
        }

        public string IP { get; private set; }

        public IPAddress IPValue { get; private set; }

        public readonly static ServiceProcess ServiceInstance = new ServiceProcess();

        void GetSetting(string fileName)
        {
            if (File.Exists(fileName) == false) return;
            var file = new FileInfo(fileName);
            var path = file.Directory.FullName;
            var setting = new ServiceSettings("setting","CoreWeb.dll", @"E:\Project\Github\LazyWelfare\src\Server\CoreWeb\CoreWeb\bin\Release\PublishOutput", 16000);
            var value = setting.GetValue();
        }
    }
}
