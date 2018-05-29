﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.ServerCore
{
    public class ServiceArg
    {
        public ServiceDefine Service { get; set; }

        public string Command { get; set; }

        public object[] Args { get; set; }
    }
}
