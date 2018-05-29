using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.PipeService
{
    public sealed class PipeServer
    {

        public const string PipeName = "LazyWelfare";

        public const string Localhost = ".";

        public static string ServerName = "";

        public static int ClientConnectTimeout = 10;


        public static void SetReaderListener(string name, int count, Action<string> call)
        {
            for (var i = 0; i < count; i++)
            {
                var listener = new PipeServerReader(name, call);
                listener.BeginWaiting();
            }
        }


        public static void WriterToListener(string name,string message)
        {
            var writer = new PipeServerWriter(name, () => message);
        }



        public class PipeMessage
        {
            public int Business { get; set; }

            public int Client { get; set; }

            public string Content { get; set; }
        }




    }



}
