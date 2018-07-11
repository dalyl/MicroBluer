namespace MicroBluer.ServerHost.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    public abstract class PageBase: UserControl
    {
        public Decorator Owner { get; protected set; }

        public PageBase()
        {

        }
    }
}
