using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.NamedPipe
{
    public sealed class PipeServer
    {
        public IServiceDistributor Switcher { get; }

        public PipeServer(IServiceDistributor switcher)
        {
            Switcher = switcher;
            SetReaderListener(PipeConfig.PipeWithResultName, 5, ReceiveWithResult);
            SetReaderListener(PipeConfig.PipeName, 5, new Action<string>(Receive));
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

        void SetReaderListener(string name, int count, Action<string> call)
        {
            for (var i = 0; i < count; i++)
            {
                var listener = new PipeServerReader(name, call);
                listener.BeginWaiting();
            }
        }

        void SetReaderListener(string name, int count, Func<string, string> call)
        {
            for (var i = 0; i < count; i++)
            {
                var listener = new PipeServerReader(name, call);
                listener.BeginWaiting();
            }
        }


        static void WriterToListener(string name, string message)
        {
            var writer = new PipeServerWriter(name, () => message);
        }

    }
}
