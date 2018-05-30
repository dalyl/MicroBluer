using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LazyWelfare.ServerCore.NamedPipe
{
    public sealed class PipeServer
    {
        public IServiceDistributor Switcher { get; }

        //    List<PipeServerReader> Listeners { get; } = new List<PipeServerReader>();

        public PipeServer(IServiceDistributor switcher)
        {
            Switcher = switcher;
            SetReaderListener(PipeConfig.PipeWithResultName, ReceiveWithResult);
            SetReaderListener(PipeConfig.PipeName, new Action<string>(Receive));
        }

        void Receive(string data)
        {
            var model = JsonConvert.DeserializeObject<ServiceArg>(data);
            var service = Switcher.GetService(model.Service);
            service.Execute(model.Command, model.Args);
        }

        string ReceiveWithResult(string data)
        {
            var model = JsonConvert.DeserializeObject<ServiceArg>(data);
            var service = Switcher.GetService(model.Service);
            var result = service.ExecuteResult(model.Command, model.Args);
            return JsonConvert.SerializeObject(result);
        }

        void SetReaderListener(string name, Action<string> call)
        {
            for (var i = 0; i < PipeConfig.MaxPipe; i++)
            {
                var listener = new PipeServerReader(name, call);
                listener.BeginWaiting();
                //   Listeners.Add(listener);
            }
        }

        void SetReaderListener(string name, Func<string, string> call)
        {
            for (var i = 0; i < PipeConfig.MaxPipe; i++)
            {
                var listener = new PipeServerReader(name, call);
                listener.BeginWaiting();
                //   Listeners.Add(listener);
            }
        }


        static void WriterToListener(string name, string message)
        {
            var writer = new PipeServerWriter(name, () => message);
        }

    }
}
