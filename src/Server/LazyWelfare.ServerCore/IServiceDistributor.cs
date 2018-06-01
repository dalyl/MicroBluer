using LazyWelfare.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.ServerCore
{
    public interface IServiceDistributor
    {
       IService GetService(ServiceDefine define);
    }
   
}
