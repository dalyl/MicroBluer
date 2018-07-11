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
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : UserControl
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private void Button_SelectServiceFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".dll";
            ofd.Filter = "dll file|*.dll";
            if (ofd.ShowDialog() == true && string.IsNullOrEmpty(ofd.FileName) == false)
            {
                var file = ofd.FileName;
                var index = file.LastIndexOf("\\");
                ServicePath.Text = file.Substring(0, index);
                ServiceName.Text = file.Substring(index + 1);
            }
        }


        private void Button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {


        }
    }
}
