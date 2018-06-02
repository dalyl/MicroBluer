using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWelfare.Interface
{
    public interface IArgument
    {
        string Name { get; set; }

        string Service { get; set; }

        string Uri { get; set; }

    }
}
