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

namespace LazyWelfare.ServerHost
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


        /// <summary>
        /// 托盘右击菜单--上线按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OnlineClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示上线！");
        }

        /// <summary>
        /// 托盘右击菜单--离开按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_LeaveClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示暂时离开！");
        }
        /// <summary>
        /// 托盘右击菜单--在忙按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_BusyClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示在忙！");
        }
        /// <summary>
        /// 托盘右击菜单--请勿打扰按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_NoBotherClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示请勿打扰！");
        }
        /// <summary>
        /// 托盘右击菜单--隐身按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_HideClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示隐身！");
        }
        /// <summary>
        /// 托盘右击菜单--离线按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OffLineClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示离线！");
        }
        /// <summary>
        /// 托盘右击菜单--关于我们按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_AboutUsClick(object sender, EventArgs e)
        {
            MessageBox.Show("提示关于我们！");
        }
        

        private void MenuItem_ExitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_MouseMove_1(object sender, MouseEventArgs e)
        {
        }

        //以下代码想实现即时聊天的记住密码功能，度娘说要存入配置文件，没搞成功，希望看到的人帮我这个忙~
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //点击按钮往配置文件存入信息
            //Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //cfa.AppSettings.Settings["LogName"].Value = "123";
            //cfa.Save();
            //MessageBox.Show(ConfigurationManager.AppSettings["LogName"]);
        }

        /// <summary>
        /// 托盘小图标的双击事件--最小化的状态下双击还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotificationAreaIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton==MouseButton.Left)
            //{
            if (this.WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            this.Show();
            this.Activate();
            // }
        }

        //退出提示程序
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

    }
}
