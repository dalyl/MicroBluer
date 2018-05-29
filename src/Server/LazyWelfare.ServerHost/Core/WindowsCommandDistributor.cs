﻿using LazyWelfare.ServerCore;
using LazyWelfare.ServerCore.CommandInterface;
using LazyWelfare.ServerHost.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyWelfare.ServerHost.Core
{
    public class WindowsCommandDistributor : IServiceDistributor
    {
        public ICommandService GetService(ServiceDefine define)
        {
            switch (define)
            {
                case ServiceDefine.Volume:return new WindowsVolume();
            }
            return null;
        }
    }
}
