﻿using Common.Logging;
using MicroBluer.ServerHost.Service;
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

namespace MicroBluer.ServerHost.Pages
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
            ServiceProcess.Instance.OpenBrower();
        }

        void Refresh()
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