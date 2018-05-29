using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.NamedPipe
{
    public class PipeServerReader
    {
        public string Name { get; }

        public Action<string> Invoke { get; }

        public Func<string,string> FetchResult { get; }

        public PipeServerReader(string name, Action<string> execute)
        {
            Name = name;
            Invoke = execute;
        }

        public PipeServerReader(string name, Func<string, string> fetchResult) 
        {
            Name = name;
            FetchResult = fetchResult;
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
                var reader = new StreamReader(pipe);
                var writer = new StreamWriter(pipe);
                var line = reader.ReadLine();
                if (FetchResult == null)
                {
                    Invoke(line);
                }
                else
                {
                    var result = FetchResult(line);
                    writer.WriteLine(result);
                    writer.Flush();
                }
            }
        }

    }

}
