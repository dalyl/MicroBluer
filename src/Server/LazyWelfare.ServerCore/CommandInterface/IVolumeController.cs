using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.CommandInterface
{
    public interface IVolumeController: ICommandService
    {
        decimal GetValue();

        void SetValue(decimal value);
    }
 
}
