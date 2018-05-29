using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.PipeService
{
    public class PipeServerReader
    {
        public string Name { get; }

        public Action<string> Call { get; }

        public PipeServerReader(string name, Action<string> call)
        {
            Name = name;
            Call = call;
        }

        public void BeginWaiting()
        {
            var pipe = new NamedPipeServerStream(Name, PipeDirection.InOut);
            Task.Run(() => WaitingForRead(pipe));
        }

        async Task WaitingForRead(NamedPipeServerStream pipe)
        {
            using (pipe)
            {
                await pipe.WaitForConnectionAsync();
                BeginWaiting();
                while (pipe.CanRead)
                {
                    var stream = new StreamReader(pipe);
                    var line = stream.ReadLine();
                    Call(line);
                }
            }
        }

    }

}
