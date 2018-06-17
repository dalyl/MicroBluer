namespace LazyWelfare.AndroidCtrls.FileExpler
{
    using Android.App;
    using Android.OS;
    using Android.Views;
    using Android.Widget;
    using Permission = Android.Manifest.Permission;
    using Environment = Android.OS.Environment;
    using Android.Support.V4.App;
    using Android.Support.V7.Widget;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using LazyWelfare.AndroidUtils.Acp;
    using LazyWelfare.AndroidCtrls.PopMenu;
    using LazyWelfare.AndroidUtils.Views;

    [Activity(Theme = "@style/Theme.InCallScreen")]
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

            var moreBtn = FindViewById(Resource.Id.FileExpleror_Menu);
            moreBtn.SetOnClickListener(new AnonymousOnClickListener(MoreClick));

            var closeBtn = FindViewById(Resource.Id.FileExpleror_Close);
            closeBtn.SetOnClickListener(new AnonymousOnClickListener(CloseClick));

            InitListView();
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            MenuInflater.Inflate(Resource.Menu.FileExplerorItem_Menu, menu);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            Adapter.SwitchContextMenu(item);
            return base.OnContextItemSelected(item);
        }

        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
            {
                if (!lastBackKeyDownTime.HasValue || (DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 1)))
                {
                    lastBackKeyDownTime = DateTime.Now;
                    if (AdapterBackUp() == false) return base.OnKeyDown(keyCode, e);
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public string Root { get; } = "/";
        private RecyclerView ListView { get; set; }
        private RelativeLayout EmptyView { get; set; }
        private ExplerAdapter Adapter { get; set; }
        private TextView NodeTree { get; set; }

        void InitListView()
        {
            ListView = FindViewById<RecyclerView>(Resource.Id.FileExpleror_RecyclerView);
            NodeTree = FindViewById<TextView>(Resource.Id.FileExpleror_NodeTree);
            EmptyView = FindViewById<RelativeLayout>(Resource.Id.FileExpleror_EmptyContent);
            RegisterForContextMenu(ListView);
            Adapter = new ExplerAdapter(this);
            Adapter.AfterItemsChanged += AdapterChanged;
            Adapter.SetData(Environment.RootDirectory.Path);
            ListView.SetLayoutManager(new LinearLayoutManager(this));
            ListView.SetAdapter(Adapter);
        }

        bool AdapterBackUp()
        {
            var current = new DirectoryInfo(Adapter.CurrentRoot);
            if (current.Parent.FullName == Root) {
                Toast.MakeText(this, "已经是根目录了", ToastLength.Short).Show();
                return false;
            }
            Adapter.SetData(current.Parent.FullName);
            return true;
        }

        void BackUpClick(View view) => AdapterBackUp();

        void CloseClick(View view) => Finish();

        void MoreClick(View view)
        {
            List<PopupMenuItem> menuList = new List<PopupMenuItem> {
                new PopupMenuItem
                {
                    ItemImage = Resource.Drawable.expleror_folder,
                    ItemText = "System",
                    Click= () =>  Adapter.SetData(Environment.RootDirectory.Path ),
                },
                new PopupMenuItem
                {
                    ItemImage = Resource.Drawable.expleror_folder,
                    ItemText = "存储器",
                    Click= () =>  Adapter.SetData(Environment.ExternalStorageDirectory.Path),
                },
            };
            var setting = new PopupMenuSetting { };
            PopMenu.PopupMenu.ShowPopupWindows(this, view, menuList, setting);
        }

        void AdapterChanged()
        {
            EmptyView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Visible : ViewStates.Invisible;
            ListView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Invisible : ViewStates.Visible;
            NodeTree.Text = $"目录 {Adapter.CurrentRoot.Replace("/",">")}";
        }

        

    }
}