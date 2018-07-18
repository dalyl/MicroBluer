namespace MicroBluer.ServerHost.UI
{
    using Autofac;
    using MicroBluer.Common;
    using MicroBluer.ServerHost.UI.Pages;
    using System.Windows.Controls;

    public class PageContainer : Border
    {
        static IContainer Container { get; set; }

        public static void Init(PageContainer PC)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance<PageContainer>(PC);
            builder.RegisterInstance<WebPage>(new WebPage());
            builder.RegisterInstance<StatePage>(new StatePage());
            builder.RegisterInstance<SettingPage>(new SettingPage());
            builder.RegisterInstance<LoadingPage>(new LoadingPage());
            
            Container = builder.Build();
        }

        public static PageContainer GetContainer()
        {
            return Container.Resolve<PageContainer>();
        }

        static void Open<T>() where T:PageBase
        {
            var page = Container.Resolve<T>();
            if (page == null) TryNotice.Current.Show(false, $"{typeof(T)} 未注册");
            var owner = GetContainer();
            owner.Child = page;
            page.Refresh();
        }
      

        public static bool Open(Page page,object contenxt=null)
        {
            switch (page)
            {
                case Page.State: Open<StatePage>(); return true;
                case Page.Setting: Open<SettingPage>(); return true;
                default: return false;
            }
        } 

        public static bool Open(PageRoute route)
        {
            var isOpen = Open(route.Page, route.Context);
            if (isOpen) return true;
            if(route.Context==null) return TryNotice.Current.Show(false, $"{ route.Page } 无法打开"); 
            if (route.Context is string) return Open(route.Context.ToString());
            return TryNotice.Current.Show(false, $"{ route.Page } 无法打开");
        }

        public static bool Open(string url)
        {
            var bower= Container.Resolve<WebPage>();
            bower.Open(url);
            return true;
        }

    }


}
