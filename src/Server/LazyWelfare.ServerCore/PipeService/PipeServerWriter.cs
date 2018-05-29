using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.PipeService
{
    public class PipeServerWriter
    {
        public string Name { get; }

        public Func<string> FetchMessage { get; }

        public PipeServerWriter(string name, Func<string> message)
        {
            Name = name;
            FetchMessage = message;
        }

        public void WaitingForWrite()
        {
            string message = FetchMessage();
            Task.Run(() => WaitingForWrite(message));
        }

        async Task WaitingForWrite(string message)
        {
            using (var pipe = new NamedPipeServerStream(Name, PipeDirection.InOut))
            {
                await pipe.WaitForConnectionAsync();

                var stream = new StreamWriter(pipe);

                stream.Write(message);

                stream.Flush();
            }
        }
    }

}
