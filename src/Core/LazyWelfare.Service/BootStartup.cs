using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LazyWelfare.Service
{
    public class BootStartup
    {
        public static void Register()
        {
            var message = new LanService.SignalMessage<string>("127.0.0.1".IpFromString(),7788,"Hi ,你好" );
            LanService.UdpSend(message);
        }
    }
}
