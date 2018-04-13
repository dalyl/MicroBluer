using System;
using System.Threading.Tasks;

namespace LazyWelfare.Server.Interface
{

    public class CommandDescptionAttribute : Attribute
    {
        
    }


    public class CommandStatus
    {

    }

    public interface ICommand
    {
        CommandStatus State { get; }

        Task Invoke();
    }
}
