namespace LazyWelfare.AndroidCtrls.FileExpler
{
    using Android.App;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Views;
    using Android.Widget;
    using LazyWelfare.AndroidUtils.Acp;
    using Permission = Android.Manifest.Permission;
    using Environment = Android.OS.Environment;
    using Android.Support.V7.Widget;
    using PopupMenu = Android.Support.V7.Widget.PopupMenu;
    using System.IO;
    using LazyWelfare.AndroidUtils.Views;
    using System;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public  class FileExplerorActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FileExpleror);

            Acp.getInstance(this).request(new AcpOptions.Builder()
                      .SetPermissions(Permission.WriteExternalStorage, Permission.ReadExternalStorage)
                      .Build(), new AnonymousAcpListener(ps => Toast.MakeText(this, $"权限拒绝", ToastLength.Short).Show(), InitListView));
            
            var backUpBtn = FindViewById(Resource.Id.FileExpleror_BackUp);
            backUpBtn.SetOnClickListener(new AnonymousOnClickListener(BackUpClick));

            var menuUpBtn = FindViewById(Resource.Id.FileExpleror_Menu);
            menuUpBtn.SetOnClickListener(new AnonymousOnClickListener(MenuClick));

            InitListView();
        }

        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
            {
              
                if (!lastBackKeyDownTime.HasValue||(DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 1)))
                {
                    lastBackKeyDownTime = DateTime.Now;
                    AdapterBackUp();
                }
                else
                {
                    //防止误操作
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public string Root
        {
            get
            {
                return Environment.RootDirectory.Path;
            }
        }

        private RecyclerView ListView { get; set; }
        private RelativeLayout EmptyView { get; set; }
        private ExplerAdapter Adapter { get; set; }
        private TextView NodeTree { get; set; }

        void InitListView()
        {
            NodeTree = FindViewById<TextView>(Resource.Id.FileExpleror_NodeTree);
            ListView = FindViewById<RecyclerView>(Resource.Id.FileExpleror_RecyclerView);
            EmptyView = FindViewById<RelativeLayout>(Resource.Id.FileExpleror_EmptyContent);
            Adapter = new ExplerAdapter(this);
            Adapter.AfterItemsChanged += AdapterChanged;
            Adapter.SetData(Root);
            ListView.SetLayoutManager(new LinearLayoutManager(this));
            ListView.SetAdapter(Adapter);
        }

        void AdapterBackUp()
        {
            var current = new DirectoryInfo(Adapter.CurrentRoot);
            if (current.FullName == Root) {
                Toast.MakeText(this, "已经是根目录了", ToastLength.Short).Show();
                return;
            }
            Adapter.SetData(current.Parent.FullName);
        }

        void BackUpClick(View v) => AdapterBackUp();

        void MenuClick(View v)
        {
            //// this.MenuInflater.Inflate(Resource.Menu.FileExplerorTopMainMenu,);
            
            ////创建弹出菜单     参数1(上下文)(要显示的弹出组件,我传了按钮点击事件的V);
            //var popupMenu = new PopupMenu(this, v);

            ////把定义好的menuXML资源文件填充到popupMenu当中 
            //popupMenu.MenuInflater.Inflate(Resource.Menu.FileExplerorTopMainMenu, popupMenu.Menu);
            //popupMenu.Show();
        }

        void AdapterChanged()
        {
            EmptyView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Visible : ViewStates.Invisible;
            ListView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Invisible : ViewStates.Visible;
            NodeTree.Text = $"目录>{Adapter.CurrentRoot.Replace("/",">")}";
        }

    }
}