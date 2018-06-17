using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MicroBluer.Common
{
    public class BootStartup
    {
        public static void Register()
        {
           


            var ip = LanService.GetLocalIP().IpFromString();


            //   var message = new LanService.SignalMessage<string>(("10.0.2.2").IpFromString(), 7788,"Hi,11111" );
            var message = new LanService.BroadcastMessage<string>(7788, "Hi,2222");
            LanService.UdpSend(message);
        }
    }
}
