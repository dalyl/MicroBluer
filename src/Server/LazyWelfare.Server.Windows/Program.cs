using LazyWelfare.Service;
using System;

namespace LazyWelfare.Server.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LanService.RecvThread(7788);
        }
    }
}
