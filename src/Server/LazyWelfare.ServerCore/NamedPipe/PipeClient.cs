using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace LazyWelfare.ServerCore.NamedPipe
{
    public sealed class PipeClient
    {
        public string Server { get; }
        public PipeClient(string server)
        {
            Server = server;
        }

        public void SendMessage(object data)
        {
            string message = JsonConvert.SerializeObject(data);
            using (var pipe = new NamedPipeClientStream(Server, PipeConfig.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect();
                // pipe.ReadMode = PipeTransmissionMode.Byte;
                var writer = new StreamWriter(pipe);
                var reader = new StreamReader(pipe);
                writer.AutoFlush = true;
                writer.WriteLine(message);
            }
        }

        public T ApplyMessage<T>(object data)
        {
            string message = JsonConvert.SerializeObject(data);
            using (var pipe = new NamedPipeClientStream(Server, PipeConfig.PipeWithResultName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect();
                //  pipe.ReadMode = PipeTransmissionMode.Byte;
                var writer = new StreamWriter(pipe);
                writer.AutoFlush = true;
                writer.WriteLine(message);
                pipe.WaitForPipeDrain();
                var reader = new StreamReader(pipe);
                var result = reader.ReadLine();
                if (string.IsNullOrEmpty(result)) return default(T);
                return JsonConvert.DeserializeObject<T>(result);
            }
        }
    }

}
