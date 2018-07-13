namespace MicroBluer.ServerHost.UI
{
    using MicroBluer.ServerHost.Service;
    using MicroBluer.ServerHost.UI.Pages;
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            // 启动后不现实任务栏图标！
            this.ShowInTaskbar = false;
            InitializeComponent();
            this.Title = "小蓝人服务";
            PageContainer.Init(this.CtrlsContainer);
            PageContainer.Open(Page.State);

            this.topMenu.ItemsSource = new TopMenus(this);
        }

        #region --- 窗口事件 ---

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// [X]关闭按钮
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion

        #region --- 菜单按钮事件 ---

        /// <summary>
        /// 托盘小图标的双击事件--最小化的状态下双击还原
        /// </summary>
        private void NotificationAreaIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized || this.WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            this.Show();
            this.Activate();
        }

        /// <summary>
        /// 关于
        /// </summary>
        private void MenuItem_AboutUsClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://bule.netter.site");
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void MenuItem_ExitClick(object sender, EventArgs e)
        {
            ServiceProcess.Instance.Stop(Application.Current.Shutdown);
        }

        #endregion

 

      
    }
}
