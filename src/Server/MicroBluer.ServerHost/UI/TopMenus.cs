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
            AddItem("设置", new PageRoute { Page = Page.Setting });
            AddWebItem();
            AddItem("语音服务", new PageRoute { Page = Page.Speech });
            AddItem("帮助", new PageRoute { Page = Page.Help });
            AddItem("关于", new PageRoute { Page = Page.About, Context = "blue.netter.site" });
        }

        #region  --- 查看 ---


        void AddShowItem()
        {
            var item = AddItem("查看", string.Empty);
            AddSubItem(item.Items, "服务状态", new PageRoute { Page = Page.State });
            AddSubItem(item.Items, "服务浏览", new PageRoute { Page = Page.Service });

        }

        #endregion

        #region --- 浏览 ---


        void AddWebItem()
        {
            var item = AddItem("浏览", string.Empty);
            AddSubItem(item.Items, "北大慕课", new PageRoute { Page = Page.Web, Context = "www.icourse163.org" });
            AddSubItem(item.Items, "清华慕课", new PageRoute { Page = Page.Web, Context = "www.xuetangx.com" });
            AddSubItem(item.Items, "CCTV-5", new PageRoute { Page = Page.Web, Context = "www.cctv.com" });
            AddSubItem(item.Items, "CCTV-13", new PageRoute { Page = Page.Web, Context = "www.cctv.com" });
            AddSubItem(item.Items, "优酷", new PageRoute { Page = Page.Web, Context = "www.youku.com" });
            AddSubItem(item.Items, "乐视", new PageRoute { Page = Page.Web, Context = "www.le.com" });
        }


        #endregion


        private void AddSubItem(ItemCollection Items, string Name, PageRoute route)
        {
            AddSubItem(Items, Name, route.ToJson());
        }

        private void AddSubItem(ItemCollection Items, string Name, string Parameter)
        {
            var item = new MenuItem { Header = Name, CommandParameter = Parameter };
            item.Click += MenuItemSwitchClick;
            Items.Add(item);
        }

        private MenuItem AddItem(string Name, PageRoute route = null)
        {
            return AddItem(Name, route == null ? "" : route.ToJson());
        }

        private MenuItem AddItem(string Name, string Parameter)
        {
            var item = new MenuItem { Header = Name, CommandParameter = Parameter };
            if (string.IsNullOrEmpty(Parameter) == false) item.Click += MenuItemSwitchClick;
            this.Add(item);
            return item;
        }

        private void MenuItemSwitchClick(object sender, EventArgs e)
        {
            var r = e as RoutedEventArgs;
            if (r == null) return;
            var menu = r.Source as MenuItem;
            if (menu == null) return;
            var param = menu.CommandParameter;
            if (param == null) return;
            var route = param.FromJson<PageRoute>();
            PageContainer.Open(route);
        }

    }
}
