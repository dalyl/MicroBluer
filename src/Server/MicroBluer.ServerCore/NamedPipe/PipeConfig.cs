using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBluer.ServerCore.NamedPipe
{
    public class PipeConfig
    {
        public const string PipeWithResultName = "MicroBluer$PipeWithResult";
        public const string PipeName = "MicroBluer$Pipe";
        public const string Localhost = ".";
        public static int MaxPipe =5;
        public static int ClientConnectTimeout = 10;
    }
}
