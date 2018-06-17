namespace MicroBluer.AndroidCtrls.FileSelect
{
    using System;
    using System.Linq;
    using System.IO;
    using Android.App;
    using Android.OS;
    using Android.Support.V4.App;
    using Android.Views;
    using Android.Support.V7.Widget;
    using Android.Widget;
    using MicroBluer.AndroidUtils.Acp;
    using Environment = Android.OS.Environment;
    using Permission = Android.Manifest.Permission;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class FileSelectorActivity : FragmentActivity
    {
        public static bool IsSelectMany { get; set; } = false;

        public static SelectorType SelectorType { get; set; } =  SelectorType.Directory;

        public static event Action OnCanceled;
        public static event Action OnCancelRequested;
        public static event Action<SelectorResult> OnSelectorCompleted;

        public static void RequestCancel()
        {
            OnCancelRequested?.Invoke();
        }

        SelectorAdapter Adapter { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FileSelector);

            Acp.getInstance(this).request(new AcpOptions.Builder()
                      .SetPermissions(Permission.WriteExternalStorage, Permission.ReadExternalStorage)
                      .Build(), new AnonymousAcpListener(ps => Toast.MakeText(this, $"权限拒绝", ToastLength.Short).Show(), InitListView));

            var sureBtn = FindViewById<TextView>(Resource.Id.FolderSelector_btSure);
            sureBtn.Click += (s, e) => SureSelect();

            var cancelBtn = FindViewById<TextView>(Resource.Id.FolderSelector_btCancel);
            cancelBtn.Click += (s, e) => CancelSelect();

            var backBtn = FindViewById(Resource.Id.FolderSelector_Back);
            backBtn.Click += (s, e) => AdapterBackUp();

            OnCancelRequested += HandleCancelSelect;

         
            InitListView();
        }

        //void BackClick()
        //{
        //    if (Adapter.ItemCount == 0)
        //    {
        //        CancelSelect();
        //        return;
        //    }
        //    if (string.IsNullOrEmpty(Adapter.CurrentRoot)) {
        //        CancelSelect();
        //        return;
        //    }
        //    var dir = new DirectoryInfo(Adapter.CurrentRoot);
        //    Adapter.SetData(dir.Parent.FullName);
        //}

        bool AdapterBackUp()
        {
            var current = new DirectoryInfo(Adapter.CurrentRoot);
            if (current.Parent.FullName == Environment.RootDirectory.Path)
            {
                Toast.MakeText(this, "已经是根目录了", ToastLength.Short).Show();
                return false;
            }
            Adapter.SetData(current.Parent.FullName);
            return true;
        }

        void SureSelect()
        {
            Finish();
            if (Adapter != null) {
                var result = new SelectorResult
                {
                    SelectItem = Adapter.Selects.FirstOrDefault(),
                    SelectItems = Adapter.Selects.ToArray(),
                };
                OnSelectorCompleted?.Invoke(result);
            }
        }

        void InitListView()
        {
            var listView = FindViewById<RecyclerView>(Resource.Id.FolderSelector_RecyclerView);
            var titleView = FindViewById<TextView>(Resource.Id.FolderSelector_Title);
            RegisterForContextMenu(listView);
            Adapter = new SelectorAdapter(this, SelectorType,IsSelectMany, Environment.ExternalStorageDirectory.Path);
            Adapter.AfterChanged += item => {
                titleView.Text = item == null ? "未知" : (new DirectoryInfo(item.Parent)).Name;
            };
            listView.SetLayoutManager(new LinearLayoutManager(this));
            listView.SetAdapter(Adapter);
        }

        void HandleCancelSelect()
        {
            this.CancelSelect();
        }

        protected override void OnDestroy()
        {
            OnCancelRequested -= HandleCancelSelect;
            base.OnDestroy();
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            switch (keyCode)
            {
                case Keycode.Back:
                    CancelSelect();
                    break;
                case Keycode.Focus:
                    return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        public void CancelSelect()
        {
            Finish();
            FileSelectorActivity.OnCanceled?.Invoke();
        }
    }
}