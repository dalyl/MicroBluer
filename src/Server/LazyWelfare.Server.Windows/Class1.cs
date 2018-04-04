using LazyWelfare.Server.Interface;
using System;
using System.Threading.Tasks;

namespace LazyWelfare.Server.Windows
{
    public class Close : ICommand
    {
        public Close() { State = new CommandStatus(); }

        public CommandStatus State { get; }

        public async Task Invoke()
        {
            
        }
    }
}
