namespace MicroBluer.AndroidCtrls.FileExpler
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
    using MicroBluer.AndroidUtils.Acp;
    using MicroBluer.AndroidCtrls.PopMenu;
    using MicroBluer.AndroidUtils.Views;
    using MicroBluer.AndroidUtils;
    using Resource = MicroBluer.AndroidCtrls.Resource;
    using Android.Content;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public  class FileExplerorActivity : FragmentActivity
    {

        public static Action<FileExplerorActivity> OnCreateStart;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FileExpleror);

            OnCreateStart?.Invoke(this);

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
            CreateItemContextMenu(menu, v, menuInfo);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            SwitchItemContextMenu(item);
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
                    if (BackUp() == false) return base.OnKeyDown(keyCode, e);
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        private RecyclerView ListView { get; set; }

        private RelativeLayout EmptyView { get; set; }

        private TextView NodeTree { get; set; }

        internal List<string> Roots { get; set; } = new List<string> { Environment.RootDirectory.Path, Environment.ExternalStorageDirectory.Path };

        protected ExplerAdapter Adapter { get; private set; }

        internal string[] Extensions { get; set; } = null;

        void InitListView()
        {
            ListView = FindViewById<RecyclerView>(Resource.Id.FileExpleror_RecyclerView);
            NodeTree = FindViewById<TextView>(Resource.Id.FileExpleror_NodeTree);
            EmptyView = FindViewById<RelativeLayout>(Resource.Id.FileExpleror_EmptyContent);
            RegisterForContextMenu(ListView);
            Adapter = new ExplerAdapter(this);
            Adapter.Extensions = Extensions;
            Adapter.AfterItemsChanged += AdapterChanged;
            Adapter.ItemClick += ItemClick;
            Adapter.SetData(Roots);
            ListView.SetLayoutManager(new LinearLayoutManager(this));
            ListView.SetAdapter(Adapter);
        }

        bool BackUp()
        {

            if (string.IsNullOrEmpty(Adapter.CurrentRoot))
            {
                Toast.MakeText(this, "已经是根目录了", ToastLength.Short).Show();
                return false;
            }

            var current = new DirectoryInfo(Adapter.CurrentRoot);
            if (Roots.Contains(current.FullName)) {
                Adapter.SetData(Roots);
                return true;
            }

            Adapter.SetData(current.Parent.FullName);
            return true;
        }

        void BackUpClick(View view) => BackUp();

        void CloseClick(View view) => Finish();

        protected virtual void MoreClick(View view)
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

        #region ---  Adapter Event  ---

        protected virtual void ItemClick(ExplerItem item)
        {
            if (item.IsDirectory)
            {
                Adapter.SetData(item.FullName);
            }
            else
            {
                TryCatch.Current.Show($"{item.Name}不是文件夹");
            }
        }

        protected virtual void AdapterChanged()
        {
            EmptyView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Visible : ViewStates.Invisible;
            ListView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Invisible : ViewStates.Visible;
            NodeTree.Text = $"目录 {Adapter.CurrentRoot.Replace("/",">")}";
        }

        #endregion

        #region ---  ItemContextMenu  ---

        public virtual void CreateItemContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            MenuInflater.Inflate(Resource.Menu.FileExplerorItem_Menu, menu);
        }

        public virtual void SwitchItemContextMenu(IMenuItem item)
        {
            var position = Adapter.SelectedPosition;
            var Items = Adapter.Items;

            if (position == -1) return;
            if (Items.Count < position) return;

            var selected = Items[position];
            if (selected == null) return;

            Adapter.SelectedPosition = -1;

            if (item.ItemId == Resource.Id.FileExplerorItem_Menu_Copy)
            {
                ClipboardManager manager = (ClipboardManager)GetSystemService(ClipboardService);
                manager.Text = selected.FullName;
                TryCatch.Current.Show($"{selected.Name}的路径已复制到剪切板");
                return;
            }

            if (item.ItemId == Resource.Id.FileExplerorItem_Menu_Rename)
            {
                var edit = new EditText(this)
                {
                    Text = selected.Name
                };
                var editDialog = new AlertDialog.Builder(this);
                editDialog.SetTitle($"{selected.Name}重命名");
                editDialog.SetIcon(selected.Icon);
                //设置dialog布局
                editDialog.SetView(edit);
                //设置按钮
                editDialog.SetPositiveButton("确定", (sender, args) => RenameClick(selected, edit.Text));
                editDialog.Create().Show();
                return;
            }

        }

        void RenameClick(ExplerItem item, string text)
        {
            var last = item.FullName.LastIndexOf(item.Name);
            var path = item.FullName.Substring(0, last);
            var newPath = $"{path}{text}";
            TryCatch.Current.Invoke(() => File.Move(item.FullName, newPath));
        }

        #endregion
    }
}