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
        public PageContainer Owner => PageContainer.GetContainer();

        public virtual void Refresh() { }


    }
}
