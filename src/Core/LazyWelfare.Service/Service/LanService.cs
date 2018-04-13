
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LazyWelfare.Service
{
    public class LanService
    {
        public class SignalMessage<T>
        {
            public SignalMessage(IPAddress newAddress, int port, T msg)
            {
                Address = newAddress;
                Port = port;
                Body = msg;
            }

            public SignalMessage(long newAddress, int port, T msg) : this(new IPAddress(newAddress), port, msg) { }

            public SignalMessage(SignalMessage<T> FailMessage, Exception ex) : this(FailMessage.Address, FailMessage.Port, FailMessage.Body)
            {
                Exception = ex;
            }


            public T Body { get; set; }

            public IPAddress Address { get; set; }

            public int Port { get; set; }

            public Exception Exception { get; set; }
        }


        public class BroadcastMessage<T> : SignalMessage<T>
        {
            public BroadcastMessage(int port, T msg) :base(IPAddress.Broadcast,port, msg) { }
        }


        public static void SocketSend<T>(SignalMessage<T> Message)
        {
            if (Message.Body == null) return;
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(Message.Address, Message.Port);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            var msg = JsonConvert.SerializeObject(Message.Body);
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            sock.SendTo(buffer, iep);
            sock.Close();
        }


        public static void UdpSend<T>(SignalMessage<T> Message)
        {
            if (Message.Body == null) return;
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            IPEndPoint endpoint = new IPEndPoint(Message.Address, Message.Port);
            var msg = JsonConvert.SerializeObject(Message.Body);
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            client.Send(buffer, buffer.Length, endpoint);
            client.Close();
        }

        public static SignalMessage<T> Receive<T>(int port)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                using (UdpClient udpClient = new UdpClient(port))
                {
                    while (true)
                    {
                        Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                        string returnData = Encoding.UTF8.GetString(receiveBytes);
                        var Body = JsonConvert.DeserializeObject<T>(returnData);
                        var msg = new SignalMessage<T>(RemoteIpEndPoint.Address, RemoteIpEndPoint.Port, Body);
                    }
                }
            }
            catch (Exception ex)
            {
                return new SignalMessage<T>(RemoteIpEndPoint.Address, RemoteIpEndPoint.Port,default(T)) { Exception = ex };
            }
        }


        public static void RecvThread(int port)
        {  
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, port));  
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);  
            while (true)  
            {  
                byte[] buf = client.Receive(ref endpoint);  
                string msg = Encoding.Default.GetString(buf);  
                Console.WriteLine(msg);  
            }  
        }  
    }
}
