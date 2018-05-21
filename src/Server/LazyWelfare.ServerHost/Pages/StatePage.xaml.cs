using Common.Logging;
using LazyWelfare.ServerHost.Service;
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

namespace LazyWelfare.ServerHost.Pages
{
    /// <summary>
    /// StatePage.xaml 的交互逻辑
    /// </summary>
    public partial class StatePage : UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger<StatePage>();

    
        public StatePage()
        {
            InitializeComponent();
            QrCode.Source = ServiceProcess.Instance.CreateImageSource(250, 250);
        }

        private void Button_ServiceStart_Click(object sender, RoutedEventArgs e)
        {
            ServiceProcess.Instance.Start();
            QrCode.Source = ServiceProcess.Instance.CreateImageSource(250, 250);
        }

        private void Button_ServiceStop_Click(object sender, RoutedEventArgs e)
        {
            ServiceProcess.Instance.Stop();
            QrCode.Source = ServiceProcess.Instance.CreateImageSource(250, 250);
        }


    }
}
