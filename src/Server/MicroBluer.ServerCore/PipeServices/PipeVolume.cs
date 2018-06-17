using MicroBluer.Interface;
using MicroBluer.ServerCore.NamedPipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroBluer.ServerCore.PipeServices
{
    public class PipeVolume : IVolumeController
    {
        PipeClient Pipe { get; }

        public PipeVolume(string server= PipeConfig.Localhost)
        {
               Pipe  = new PipeClient(server);
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
