namespace LazyWelfare.AndroidCtrls.FileExpler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Support.V4.App;
    using Android.Views;
    using Android.Widget;
    using LazyWelfare.AndroidUtils.Acp;
    using Permission = Android.Manifest.Permission;
    using Environment = Android.OS.Environment;
    using Android.Support.V7.Widget;
    using System.IO;
    using LazyWelfare.AndroidUtils.Views;

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
            Adapter.SetData(Environment.RootDirectory.Path);
            ListView.SetLayoutManager(new LinearLayoutManager(this));
            ListView.SetAdapter(Adapter);
        }

        void BackUpClick(View v)
        {
            var current =new DirectoryInfo(Adapter.CurrentRoot) ;
            if (current.FullName == Environment.RootDirectory.Path) return;
            Adapter.SetData(current.Parent.FullName);
        }

        void MenuClick(View v)
        {

        }

        void AdapterChanged()
        {
            EmptyView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Visible : ViewStates.Invisible;
            ListView.Visibility = Adapter.ItemCount == 0 ? ViewStates.Invisible : ViewStates.Visible;
            NodeTree.Text = $"目录>{Adapter.CurrentRoot.Replace("/",">")}";
        }

    }
}