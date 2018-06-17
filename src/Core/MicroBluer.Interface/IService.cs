using System;
using System.Collections.Generic;
using System.Text;

namespace MicroBluer.Interface
{
    public interface IService
    {
        void Execute(string command,object[] args);
        object ExecuteResult(string command, object[] args);
    }
}
