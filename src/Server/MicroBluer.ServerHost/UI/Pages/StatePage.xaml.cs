using Common.Logging;
namespace MicroBluer.ServerHost.UI.Pages
{
    using TryNotice= MicroBluer.Common.TryNotice;
    using MicroBluer.ServerHost.Service;
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// StatePage.xaml 的交互逻辑
    /// </summary>
    public partial class StatePage :  PageBase
    {
        private static readonly ILog logger = LogManager.GetLogger<StatePage>();
     
        public StatePage()
        {
            InitializeComponent();
        }

        public void Open()
        {
            base.
            Owner.Child = this;
            Refresh();
        }

        private void Button_ServiceStart_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            ServiceProcess.Instance.Start(Refresh);
        }

        private void Button_ServiceStop_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            ServiceProcess.Instance.Stop(Refresh);
        }

        private void Button_ServicePreview_Click(object sender, RoutedEventArgs e)
        {
            TryNotice.Current.Invoke(ServiceProcess.Instance.State,()=> ServiceProcess.Instance.OpenBrower());
        }

        public override void Refresh()
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                QrCode.Source = ServiceProcess.Instance.CreateImageSource(250, 250);
                ServiceAddress.Text = ServerEnvironment.Instance.WebAddress;
                ServiceState.Text = ServiceProcess.Instance.State ? "正运行" : "已停止";
                loading.Visibility = Visibility.Hidden;
            }));
        }

    }
}
