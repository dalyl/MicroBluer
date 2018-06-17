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

namespace MicroBluer.ServerHost
{
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
            if (this.WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
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
            Application.Current.Shutdown();
        }

        #endregion

    }
}
