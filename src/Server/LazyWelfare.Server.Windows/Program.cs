using LazyWelfare.Service;
using System;

namespace LazyWelfare.Server.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LanService.RecvThread("127.0.0.1".IpToUint(),7788);
        }
    }
}
