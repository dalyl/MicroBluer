namespace MicroBluer.ServerHost
{
    using MicroBluer.Common;
    using MicroBluer.ServerHost.Pages;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoadingPage loadingPage = new LoadingPage();
        private StatePage statePage = new StatePage();
        private SettingPage setPage = new SettingPage();
        private WebPage webPage = new WebPage();

        public MainWindow()
        {
            // 启动后不现实任务栏图标！
            this.ShowInTaskbar = false;
            InitializeComponent();
            this.Title = "小蓝人服务";
            this.CtrlsContainer.Child = statePage;
            this.TopMenu.ItemsSource = new TopMenus(this);
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
            Application.Current.Shutdown();
        }

        #endregion

        public bool ExplerorWeb(TopMenus.Route route)
        {
            if (route.Context == null) return TryNotice.Current.Show(false, $"{nameof(route.Context)}参数配置异常");
            this.CtrlsContainer.Child = webPage;
            webPage.OpenUrl(route.Context.ToString());
            return true;
        }

        public bool ExplerorRoute(TopMenus.Route route)
        {
            switch (route.Page) {
                case TopMenus.Page.State: this.CtrlsContainer.Child = statePage;return true;
                case TopMenus.Page.Setting: this.CtrlsContainer.Child = setPage; return true;

                default:return false;
            }
        }



        public class TopMenus : List<MenuItem>
        {

            public MainWindow Container { get; }

            public TopMenus(MainWindow win)
            {
                Container = win;
                InitData();
            }

            void InitData()
            {
                AddShowItem();
                AddItem("设置", new Route { Page = Page.Setting }, (route) => Container.ExplerorRoute(route));
                AddWebItem();
                AddItem("帮助", new Route { Page = Page.Help }, (route) => Container.ExplerorRoute(route));
                AddItem("关于", new Route { Page = Page.About, Context="blue.netter.site" }, (route) => Container.ExplerorWeb(route));
            }

            #region  --- 查看 ---


            void AddShowItem()
            {
                var item = AddItem("查看", string.Empty, (route) => Container.ExplerorRoute(route));
                AddSubItem(item.Items, "运行状态", new Route { Page = Page.State });
            }


            #endregion

            #region --- 浏览 ---


            void AddWebItem()
            {
                var item = AddItem("浏览", string.Empty, (route) => Container.ExplerorWeb(route));
                AddSubItem(item.Items, "CCTV-5", new Route { Page = Page.Web, Context = "www.cctv.com" });
                AddSubItem(item.Items, "CCTV-13", new Route { Page = Page.Web, Context = "www.cctv.com" });
                AddSubItem(item.Items, "优酷", new Route { Page = Page.Web, Context = "www.youku.com" });
                AddSubItem(item.Items, "乐视", new Route { Page = Page.Web, Context = "www.le.com" });
            }


            #endregion


            private void AddSubItem(ItemCollection Items, string Name, Route route)
            {
                AddSubItem(Items, Name, route.ToJson());
            }

            private void AddSubItem(ItemCollection Items, string Name, string Parameter)
            {
                var item = new MenuItem { Header = Name, CommandParameter = Parameter };
                Items.Add(item);
            }

            private MenuItem AddItem(string Name, Route route = null, Action<Route> RouteInvoke = null)
            {
                return AddItem(Name, route == null ? "" : route.ToJson(), RouteInvoke);
            }

            private MenuItem AddItem(string Name, string Parameter, Action<Route> RouteInvoke)
            {
                var item = new MenuItem { Header = Name, CommandParameter = Parameter };
                item.Click += (s, e) => MenuItemSwitchClick(s, e, RouteInvoke);
                this.Add(item);
                return item;
            }

            private void MenuItemSwitchClick(object sender, EventArgs e, Action<Route> RouteInvoke)
            {
                var r = e as RoutedEventArgs;
                if (r == null) return;
                var menu = r.Source as MenuItem;
                if (menu == null) return;
                var param = menu.CommandParameter;
                if (param == null) return;
                var route = param.FromJson<Route>();
                RouteInvoke?.Invoke(route);
            }

            public class Route
            {
                public Page Page { get; set; }

                public object Context { get; set; }
            }

            public enum Page
            {
                None,
                State,
                Setting,
                Web,
                Help,
                About,
            }



        }
    }
}
