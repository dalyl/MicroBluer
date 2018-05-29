using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore.Command
{
    public interface IVolumeControl
    {
        decimal GetValue();

        void SetValue(decimal value);
    }
 
}
