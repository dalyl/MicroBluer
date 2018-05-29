using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.ServerCore.CommandInterface
{
    public interface ICommandService
    {
        void Execute(string command,object[] args);
        object ExecuteResult(string command, object[] args);
    }
}
