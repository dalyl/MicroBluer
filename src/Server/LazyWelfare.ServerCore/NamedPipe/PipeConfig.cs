﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.ServerCore.NamedPipe
{
    public class PipeConfig
    {
        public const string PipeWithResultName = "LazyWelfare$PipeWithResult";
        public const string PipeName = "LazyWelfare$Pipe";
        public const string Localhost = ".";
        public static int ClientConnectTimeout = 10;
    }
}
