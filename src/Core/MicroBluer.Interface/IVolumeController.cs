using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroBluer.Interface
{
    public interface IVolumeController
    {
        decimal GetValue();

        void SetValue(decimal value);
    }
 
}
