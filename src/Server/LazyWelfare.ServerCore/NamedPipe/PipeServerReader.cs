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

        public void BeginWaiting() => Task.Run(() => WaitingForRead());

        async Task WaitingForRead()
        {
            try
            {
                using (var pipe = new NamedPipeServerStream(Name, PipeDirection.InOut, PipeConfig.MaxPipe))
                {
                    await pipe.WaitForConnectionAsync();
                    BeginWaiting();
                    var reader = new StreamReader(pipe);
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) return;
                    if (FetchResult == null)
                    {
                        Invoke(line);
                    }
                    else
                    {
                        var writer = new StreamWriter(pipe);
                        var result = FetchResult(line);
                        writer.AutoFlush = true;
                        writer.WriteLine(result);
                    }
                }
                BeginWaiting();
            }
            catch {

            }
        }

    }

}
