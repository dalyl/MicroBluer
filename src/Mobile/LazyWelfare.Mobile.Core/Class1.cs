using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LazyWelfare.Mobile.Core
{
    public class LAN
    {
        public static void ccc()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, 11000);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            byte[] buffer = Encoding.UTF8.GetBytes("Hello");
            sock.SendTo(buffer, iep);
            sock.Close();
        }

        public static void ddd() {

            try
            {
                using (UdpClient udpClient = new UdpClient(11000))
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    while (true)
                    {
                        Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                        string returnData = Encoding.UTF8.GetString(receiveBytes);
                        Invoke((MethodInvoker)(() =>
                        {
                            textBox1.Text += "Receive Message : " + returnData + Environment.NewLine;
                            textBox1.Text += "Receive IP : " + RemoteIpEndPoint.Address + Environment.NewLine;
                            textBox1.Text += "Receive Port : " + RemoteIpEndPoint.Port + Environment.NewLine;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += ex.ToString();
            }
        }
    }
}
