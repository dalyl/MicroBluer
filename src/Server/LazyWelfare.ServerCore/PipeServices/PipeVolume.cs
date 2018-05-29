using LazyWelfare.ServerCore.CommandInterface;
using LazyWelfare.ServerCore.NamedPipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.PipeServices
{
    public class PipeVolume : IVolumeController
    {
        PipeClient Pipe { get; } = new PipeClient();

        void ICommandService.Execute(string command, object[] args)
        {
            throw new NotImplementedException();
        }

        object ICommandService.ExecuteResult(string command, object[] args)
        {
            throw new NotImplementedException();
        }

        public decimal GetValue()
        {
            var args = new ServiceArg
            {
                Service = ServiceDefine.Volume,
                Command = nameof(GetValue),
                Args = null
            };
            return Pipe.ApplyMessage<decimal>(args);
        }


        public void SetValue(decimal value)
        {
            var args = new ServiceArg
            {
                Service = ServiceDefine.Volume,
                Command = nameof(SetValue),
                Args =new object[]{ value }
            };
            Pipe.SendMessage(args);
        }
    }
 
}
