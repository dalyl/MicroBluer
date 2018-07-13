using MicroBluer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MicroBluer.ServerHost.UI.Pages
{
    /// <summary>
    /// WebPage.xaml 的交互逻辑
    /// </summary>
    public partial class WebPage :  PageBase
    {

        public WebPage() 
        {
            InitializeComponent();
        }

        public bool Open(TopMenus.PageRoute route)
        {
            if (route.Context == null) return TryNotice.Current.Show(false, $"{nameof(route.Context)}参数配置异常");
            Open(route.Context.ToString());
            return true;
        }

        public void Open(string url)
        {
            Owner.Child = this;
            OpenUrl(url);
        }

        public void OpenUrl(string url)
        {
            this.chromeBrowser.Address = url;
        }
    }
}
