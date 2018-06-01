using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.Interface
{
    public interface IService
    {
        void Execute(string command,object[] args);
        object ExecuteResult(string command, object[] args);
    }
}
