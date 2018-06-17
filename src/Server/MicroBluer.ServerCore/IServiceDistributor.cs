using MicroBluer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBluer.ServerCore
{
    public interface IServiceDistributor
    {
       IService GetService(ServiceDefine define);
    }
   
}
