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
    using Android.Support.V4.Widget;
    using View = Android.Views.View;
    using Android.Views;
    using LazyWelfare.AndroidUtils.Views;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity :  PartialActivity
    {
        static PartialRequestStack requestStack { get; set; } = new PartialRequestStack();

        public AgreementUri Partial { get; set; } = FolderMapsView.Partial;

        public override PartialRequestStack RequestStack { get; } = requestStack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var view = InitComponents();

            SetContentView(view);
            SetTitle(ActiveContext.User.Name);

            LoadMenu();
            LoadWebview();
        }

        public DrawerLayout MenuLayout { get; protected set; }

        public View LeftMenu { get; protected set; }

        public View RightMenu { get; protected set; }

        public View ToolBar { get; protected set; }

        View InitComponents()
        {
            var view = LayoutInflater.FromContext(this).Inflate(Resource.Layout.MenuLayout, null);
            MenuLayout = view.FindViewById<DrawerLayout>(Resource.Id.menu_layout);
            LeftMenu = MenuLayout.FindViewById<LinearLayout>(Resource.Id.MenuLeftContent);
            RightMenu = MenuLayout.FindViewById<RelativeLayout>(Resource.Id.MenuRightContent);
            ToolBar = MenuLayout.FindViewById(Resource.Id.MenuToolBar);

            LeftMenu.Clickable = true;
            RightMenu.Clickable = true;

            var leftBtn = ToolBar.FindViewById(Resource.Id.toolbar_left);
            var rightBtn = ToolBar.FindViewById(Resource.Id.toolbar_right);

            leftBtn.Click += (o, e) => MenuLayout.OpenDrawer(LeftMenu);
            rightBtn.Click += (o, e) => MenuLayout.OpenDrawer(RightMenu);

            var panel = MenuLayout.FindViewById<LinearLayout>(Resource.Id.MenuMainPanel);
            PartialView = new WebView(this);
            panel.AddView(PartialView);
            return view;
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

            var intColor = Resource.Color.MenuMainPanel_Background;
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
            var listView = (ListView)MenuLayout.FindViewById(Resource.Id.MenuLeft_ListView);
            var data = new MenuContentItem[]
            {
                new MenuContentItem(Resource.Drawable.base_home_black, "首页", 1,()=>OpenWebview(FolderMapsView.Partial)),
                new MenuContentItem(Resource.Drawable.base_folder_black, "资源归档", 2,()=>OpenWebview(FolderMapsView.Partial)),
                new MenuContentItem(Resource.Drawable.base_cloud_black, "主机服务", 3,()=>OpenWebview(HostIndexView.Partial)),
                new MenuContentItem(Resource.Drawable.base_qrcode_black, "扫一扫", 4),
                new MenuContentItem(Resource.Drawable.base_cast_connected_black, "联机服务", 5),
            };
            var adapter = new MenuContentAdapter(this, data);
            listView.Adapter = adapter;
            listView.OnItemClickListener = new ItemClickListener((adpter, view, position) => data[position].Click());
            //去除行与行之间的黑线：  
            listView.Divider=(null);
        }

        void OpenWebview(AgreementUri uri)
        {
            MenuLayout.CloseDrawers();
            PartialView.EvaluateJavascript($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{uri.Host}','{uri.Path}',null);", null);
        }

    }


}