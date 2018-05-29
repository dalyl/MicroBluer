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
        public void SendMessage(object data)
        {
            string message = JsonConvert.SerializeObject(data);
            using (var pipe = new NamedPipeClientStream(PipeConfig.Localhost, PipeConfig.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect();
                pipe.ReadMode = PipeTransmissionMode.Message;
                var writer = new StreamWriter(pipe);
                var reader = new StreamReader(pipe);
                writer.Write(message);
                writer.Flush();
            }
        }

        public T ApplyMessage<T>(object data)
        {
            string message = JsonConvert.SerializeObject(data);
            using (var pipe = new NamedPipeClientStream(PipeConfig.Localhost, PipeConfig.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect();
                pipe.ReadMode = PipeTransmissionMode.Message;
                var writer = new StreamWriter(pipe);
                var reader = new StreamReader(pipe);
                writer.Write(message);
                writer.Flush();
                pipe.WaitForPipeDrain();
                var result = reader.ReadLine();
                return JsonConvert.DeserializeObject<T>(result);
            }
        }
    }

}
