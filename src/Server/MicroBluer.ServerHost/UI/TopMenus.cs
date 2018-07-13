namespace MicroBluer.ServerHost.UI
{
    using MicroBluer.Common;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

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
            AddItem("设置", new PageRoute { Page = Page.Setting }, (route) => Container.ExplerorRoute(route));
            AddWebItem();
            AddItem("帮助", new PageRoute { Page = Page.Help }, (route) => Container.ExplerorRoute(route));
            AddItem("关于", new PageRoute { Page = Page.About, Context = "blue.netter.site" }, (route) => Container.WebPage.Open(route));
        }

        #region  --- 查看 ---


        void AddShowItem()
        {
            var item = AddItem("查看", string.Empty, (route) => Container.ExplerorRoute(route));
            AddSubItem(item.Items, "服务状态", new PageRoute { Page = Page.State });
            AddSubItem(item.Items, "服务浏览", new PageRoute { Page = Page.Service }, (route) => Container.ExplerorService(route));

        }

        #endregion

        #region --- 浏览 ---


        void AddWebItem()
        {
            var item = AddItem("浏览", string.Empty, (route) => Container.WebPage.Open(route));
            AddSubItem(item.Items, "北大慕课", new PageRoute { Page = Page.Web, Context = "www.icourse163.org" });
            AddSubItem(item.Items, "清华慕课", new PageRoute { Page = Page.Web, Context = "www.xuetangx.com" });
            AddSubItem(item.Items, "CCTV-5", new PageRoute { Page = Page.Web, Context = "www.cctv.com" });
            AddSubItem(item.Items, "CCTV-13", new PageRoute { Page = Page.Web, Context = "www.cctv.com" });
            AddSubItem(item.Items, "优酷", new PageRoute { Page = Page.Web, Context = "www.youku.com" });
            AddSubItem(item.Items, "乐视", new PageRoute { Page = Page.Web, Context = "www.le.com" });
        }


        #endregion


        private void AddSubItem(ItemCollection Items, string Name, PageRoute route, Action<PageRoute> RouteInvoke = null)
        {
            AddSubItem(Items, Name, route.ToJson(), RouteInvoke);
        }

        private void AddSubItem(ItemCollection Items, string Name, string Parameter, Action<PageRoute> RouteInvoke = null)
        {
            var item = new MenuItem { Header = Name, CommandParameter = Parameter };
            if (RouteInvoke != null) item.Click += (s, e) => MenuItemSwitchClick(s, e, RouteInvoke);
            Items.Add(item);
        }

        private MenuItem AddItem(string Name, PageRoute route = null, Action<PageRoute> RouteInvoke = null)
        {
            return AddItem(Name, route == null ? "" : route.ToJson(), RouteInvoke);
        }

        private MenuItem AddItem(string Name, string Parameter, Action<PageRoute> RouteInvoke)
        {
            var item = new MenuItem { Header = Name, CommandParameter = Parameter };
            item.Click += (s, e) => MenuItemSwitchClick(s, e, RouteInvoke);
            this.Add(item);
            return item;
        }

        private void MenuItemSwitchClick(object sender, EventArgs e, Action<PageRoute> RouteInvoke)
        {
            var r = e as RoutedEventArgs;
            if (r == null) return;
            var menu = r.Source as MenuItem;
            if (menu == null) return;
            var param = menu.CommandParameter;
            if (param == null) return;
            var route = param.FromJson<PageRoute>();
            RouteInvoke?.Invoke(route);
        }

    }
}
