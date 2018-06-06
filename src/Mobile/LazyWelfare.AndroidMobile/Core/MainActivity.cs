namespace LazyWelfare.AndroidMobile
{
    using LazyWelfare.AndroidMobile.Views;
    using LazyWelfare.AndroidMobile.Views.Partials;
    using LazyWelfare.AndroidMobile.Script;
    using LazyWelfare.AndroidMobile.WebAgreement;
    using Android.App;
    using Android.OS;
    using Android.Webkit;
    using Android.Widget;
    using System.Collections.Generic;
    using Android.Support.V4.Widget;
    using View = Android.Views.View;
    using Android.Views;

    //   using Toolbar = Android.Support.V7.Widget.Toolbar;
    //   using Android.Support.V7.App;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity :  PartialActivity
    {
        static PartialRequestStack requestStack { get; set; } = new PartialRequestStack();

        public AgreementUri Partial { get; set; } = HomeView.Partial;

        public override PartialRequestStack RequestStack { get; } = requestStack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = LayoutInflater.FromContext(this).Inflate(Resource.Layout.MenuLayout, null);
            InitView(view);
            // Set our view from the "main" layout resource

            SetContentView(view);
            SetTitle(ActiveContext.User.Name);

            LoadMenu();
            LoadWebview();
        }

        public DrawerLayout MenuLayout { get; protected set; }
        public View LeftMenu { get; protected set; }
        public View RightMenu { get; protected set; }

        public View ToolBar { get; protected set; }

        void InitView(View view)
        {
            MenuLayout = view.FindViewById<DrawerLayout>(Resource.Id.menu_layout);
            LeftMenu = MenuLayout.FindViewById(Resource.Id.left);
            RightMenu = MenuLayout.FindViewById(Resource.Id.right);
            ToolBar = MenuLayout.FindViewById(Resource.Id.toolBar);

            var leftBtn = ToolBar.FindViewById(Resource.Id.toolbar_left);
            var rightBtn = ToolBar.FindViewById(Resource.Id.toolbar_right);

            leftBtn.Click += (o, e) => MenuLayout.OpenDrawer(LeftMenu);
            rightBtn.Click += (o, e) => MenuLayout.OpenDrawer(RightMenu);

            var iv_main = MenuLayout.FindViewById<LinearLayout>(Resource.Id.iv_main);
            PartialView = new WebView(this);
            iv_main.AddView(PartialView);
        }

        void LoadWebview()
        {

            WebSettings settings = PartialView.Settings;

            //启用js事件  
            settings.SetSupportZoom(true);
            settings.JavaScriptEnabled = true;
            //启用js的dom缓存  
            settings.DomStorageEnabled = true;
            //加载javascript接口方法，以便调用前台方法  
            PartialView.AddJavascriptInterface(new AndroidScript(this, PartialView), "AndroidScript");
            PartialView.AddJavascriptInterface(new PartialScript(this, PartialView), "PartialScript");
            PartialView.AddJavascriptInterface(new BuinessScript(this, PartialView), "BuinessScript");
            PartialView.SetWebViewClient(new AgreementRouteClient($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{Partial.Host}','{Partial.Path}',null);"));

            var intColor = Resource.Color.iv_main_Background;
            var color = GetColor(intColor);
            var setting = new Template.Setting {
                Background= string.Format("#%06X", 0xFFFFFF & color),
            };

            PartialView.LoadDataWithBaseURL("file:///android_asset/", Template.Layout(setting), "text/html", "UTF-8", null);
        }

        void SetTitle(string title)
        {
            var titleBar = ToolBar.FindViewById<TextView>(Resource.Id.toolbar_Title);
            titleBar.Text = title;
        }

        void LoadMenu()
        {
            var listView = (ListView)MenuLayout.FindViewById(Resource.Id.left_listview);
            var data = new List<MenuContentItem>
            {
                new MenuContentItem(Resource.Drawable.ic_launcher_round, "新闻", 1),
                new MenuContentItem(Resource.Drawable.ic_launcher_round, "订阅", 2),
                new MenuContentItem(Resource.Drawable.ic_launcher_round, "图片", 3),
                new MenuContentItem(Resource.Drawable.ic_launcher_round, "视频", 4),
                new MenuContentItem(Resource.Drawable.ic_launcher_round, "跟帖", 5),
                new MenuContentItem(Resource.Drawable.ic_launcher_round, "投票", 6)
            };
            var adapter = new MenuContentAdapter(this, data);
            listView.Adapter = adapter;
        }

        public override void ShowLeftMenu(string args)
        {
            RunOnUiThread(() => {
                MenuLayout.CloseDrawers();
                MenuLayout.OpenDrawer(LeftMenu);
            });
        }

        public override void ShowRightMenu(string args)
        {
            RunOnUiThread(() =>
            {
                MenuLayout.CloseDrawers();
                MenuLayout.OpenDrawer(RightMenu);
            });
        }


    }




}