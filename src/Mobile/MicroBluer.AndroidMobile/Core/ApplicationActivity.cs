namespace MicroBluer.AndroidMobile
{
    using MicroBluer.AndroidMobile.Views;
    using MicroBluer.AndroidMobile.Views.Partials;
    using MicroBluer.AndroidMobile.Script;
    using MicroBluer.AndroidMobile.WebAgreement;
    using System;
    using Android.App;
    using Android.OS;
    using Android.Webkit;
    using Android.Widget;
    using Android.Support.V4.Widget;
    using View = Android.Views.View;
    using Android.Views;
    using MicroBluer.AndroidUtils.Views;
    using MicroBluer.AndroidMobile.Models;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class ApplicationActivity :  PartialActivity
    {
        static PartialRequestStack requestStack { get; set; } = new PartialRequestStack();

        public AgreementUri Partial { get; set; } = UserIndexView.Partial;

        public ApplicationActivity() : base(requestStack) { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var view = InitComponents();

            SetContentView(view);
            SetTitle(ActiveContext.User.Name);
            LoadUser();
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

        public static void UpdateView<T>()
        {
            if (ActiveContext.Activity == null) return;
            if (ActiveContext.IsMainActivityAlive(nameof(ApplicationActivity)) == false) return;
            if ((ActiveContext.Activity is ApplicationActivity) == false) return;
            var The = ActiveContext.Activity as ApplicationActivity;
            if (typeof(T) == typeof(UserModel))
            {
                The.LoadUser();
            }
        }

        void LoadUser()
        {
            var userHead = MenuLayout.FindViewById<ImageView>(Resource.Id.MenuLeft_UserHead);
            var userName = MenuLayout.FindViewById<TextView>(Resource.Id.MenuLeft_UserName);
            var userSignature = MenuLayout.FindViewById<TextView>(Resource.Id.MenuLeft_UserSignature);
            userName.Text = $"{Greetings},{ActiveContext.User.Name}";
            userSignature.Text = ActiveContext.User.Signature;
        }

        string Greetings
        {
            get
            {
                var hour = DateTime.Now.Hour;
                if (hour > 5 && hour < 11) return "早上好";    //6.7.8.9.10
                if (hour > 10 && hour < 14) return "中午好";   //11.12.13
                if (hour > 13 && hour < 19) return "下午好";   //14.15.16.17.18
                if (hour > 18 && hour < 23) return "晚上好";   //19.20.21.22
                if (hour > 18 && hour < 23) return "该睡了";   //23.0.1.2,3,4.5
                return "您好";
            }
        }

        void LoadMenu()
        {
            var listView = MenuLayout.FindViewById<ListView>(Resource.Id.MenuLeft_ListView);
            var data = new MenuContentItem[]
            {
                new MenuContentItem(Resource.Drawable.base_home_black, "首页", 1,()=>OpenWebview(UserIndexView.Partial)),
                new MenuContentItem(Resource.Drawable.base_folder_black, "文档管理", 2,()=>OpenWebview(FolderMapIndexView.Partial)),
                new MenuContentItem(Resource.Drawable.base_cloud_black, "主机服务", 3,()=>OpenWebview(HostIndexView.Partial)),
                new MenuContentItem(Resource.Drawable.base_edit_black, "Hosts 编辑", 4,()=>OpenWebview(FileHostsIndexView.Partial)),
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