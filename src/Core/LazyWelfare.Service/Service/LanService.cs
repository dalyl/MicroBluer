
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LazyWelfare.Common
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


        public static void RecvThread(long ip,int port)
        {  
            UdpClient client = new UdpClient(new IPEndPoint(ip, port));  
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);  
            while (true)  
            {  
                byte[] buf = client.Receive(ref endpoint);  
                string msg = Encoding.Default.GetString(buf);  
                Console.WriteLine(msg);  
            }  
        }

        /// <summary>  
        /// 获取当前使用的IP  
        /// </summary>  
        /// <returns></returns>  
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = ((System.Net.IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>  
        /// 获取本机主DNS  
        /// </summary>  
        /// <returns></returns>  
        public static string GetPrimaryDNS()
        {
            string result = RunApp("nslookup", "", true);
            Match m = Regex.Match(result, @"\d+\.\d+\.\d+\.\d+");
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>  
        /// 运行一个控制台程序并返回其输出参数。  
        /// </summary>  
        /// <param name="filename">程序名</param>  
        /// <param name="arguments">输入参数</param>  
        /// <returns></returns>  
        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                using (System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    //string txt = sr.ReadToEnd();  
                    //sr.Close();  
                    //if (recordLog)  
                    //{  
                    //    Trace.WriteLine(txt);  
                    //}  
                    //if (!proc.HasExited)  
                    //{  
                    //    proc.Kill();  
                    //}  
                    //上面标记的是原文，下面是我自己调试错误后自行修改的  
                    Thread.Sleep(100);           //貌似调用系统的nslookup还未返回数据或者数据未编码完成，程序就已经跳过直接执行  
                                                 //txt = sr.ReadToEnd()了，导致返回的数据为空，故睡眠令硬件反应  
                    if (!proc.HasExited)         //在无参数调用nslookup后，可以继续输入命令继续操作，如果进程未停止就直接执行  
                    {                            //txt = sr.ReadToEnd()程序就在等待输入，而且又无法输入，直接掐住无法继续运行  
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }
    }
}
