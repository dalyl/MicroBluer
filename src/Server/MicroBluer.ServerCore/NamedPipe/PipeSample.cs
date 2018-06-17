using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace MicroBluer.ServerCore.NamedPipe
{
    public class PipeSample
    {
        public class Server
        {
            private NamedPipeServerStream _pipe;

            private const string PipeName = "PipeSample";

            private const int PipeInBufferSize = 4096;

            private const int PipeOutBufferSize = 65535;

            private Encoding encoding = Encoding.UTF8;

            public Server()
            {

                _pipe = new NamedPipeServerStream
                (
                    PipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                    PipeInBufferSize,
                    PipeOutBufferSize
                 );

                _pipe.BeginWaitForConnection(WaitForConnectionCallback, _pipe);
            }

            private void WaitForConnectionCallback(IAsyncResult ar)
            {
                var pipeServer = (NamedPipeServerStream)ar.AsyncState;

                pipeServer.EndWaitForConnection(ar);

                var data = new byte[PipeInBufferSize];

                var count = pipeServer.Read(data, 0, PipeInBufferSize);

                if (count > 0)
                {
                    // 通信双方可以约定好传输内容的形式，例子中我们传输简单文本信息。

                    string message = encoding.GetString(data, 0, count);


                }
            }

            private void OnSend()
            {
                if (_pipe.IsConnected)
                {
                    try
                    {
                        string message = "";

                        byte[] data = encoding.GetBytes(message);

                        _pipe.Write(data, 0, data.Length);
                        _pipe.Flush();
                        _pipe.WaitForPipeDrain();
                    }
                    catch { }
                }
            }
        }

        public class Client
        {

            private const string PipeName = "PipeSample";

            private Encoding encoding = Encoding.UTF8;

            private NamedPipeClientStream _pipe;

            private bool _starting = false;


            private void OnConnect()
            {
                if (_starting)
                {
                    return;
                }

                try
                {

                    _pipe = new NamedPipeClientStream
                    (
                        ".",
                        PipeName,
                        PipeDirection.InOut,
                        PipeOptions.Asynchronous | PipeOptions.WriteThrough
                    );

                    _pipe.Connect();

                    _pipe.ReadMode = PipeTransmissionMode.Message;

                    string message = "Connected!";

                    byte[] data = encoding.GetBytes(message);

                    _pipe.BeginWrite(data, 0, data.Length, PipeWriteCallback, _pipe);

                    _starting = true;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.StackTrace);
                }
            }

            private void PipeWriteCallback(IAsyncResult ar)
            {
                var pipe = (NamedPipeClientStream)ar.AsyncState;

                pipe.EndWrite(ar);
                pipe.Flush();
                pipe.WaitForPipeDrain();

                var data = new byte[65535];

                var count = pipe.Read(data, 0, data.Length);

                if (count > 0)
                {
                    string message = encoding.GetString(data, 0, count);


                }
            }
        }

    }
}