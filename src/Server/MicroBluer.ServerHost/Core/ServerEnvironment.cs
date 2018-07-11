using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using MicroBluer.Common;

namespace MicroBluer.ServerHost
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

        public string Path { get; private set; } = @"E:\Project\Github\MicroBluer\src\Server\ServerPulish\";

        public string WebName { get; private set; } = "MicroBluer.ServerWeb";

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
