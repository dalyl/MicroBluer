using LazyWelfare.ServerCore.CommandInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.ServerCore
{
    public  interface IServiceDistributor
    {
       ICommandService GetService(ServiceDefine define);
    }
   
}
