using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using LazyWelfare.Common;

namespace LazyWelfare.ServerHost
{
    public class ServerEnvironment
    {

        private static readonly ILog logger = LogManager.GetLogger(nameof(ServerEnvironment));

        private ServerEnvironment()
        {
            IP = LanService.GetLocalIP();
            IPValue = IP.IpFromString();
        }

        public readonly static ServerEnvironment Instance = new ServerEnvironment();

        public string Path { get; private set; } = @"E:\Project\Github\LazyWelfare\src\Server\LazyWelfare.ServerHost\bin\Debug\webroot\";

        public string WebName { get; private set; } = "LazyWelfare.ServerWeb";

        public string IP { get; private set; }

        public System.Net.IPAddress IPValue { get; private set; }

        public int Port { get;  set; } = 6001;

        public string WebAddress {
            get {
                return $"http://{IP}:{Port}";
            }
        }
   

    }
}
