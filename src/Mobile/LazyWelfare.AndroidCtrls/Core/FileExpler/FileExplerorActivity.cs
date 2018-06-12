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

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public  class FileExplerorActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FileSelector);

            Acp.getInstance(this).request(new AcpOptions.Builder()
                      .SetPermissions(Permission.WriteExternalStorage, Permission.ReadExternalStorage)
                      .Build(), new AnonymousAcpListener(ps => Toast.MakeText(this, $"权限拒绝", ToastLength.Short).Show(), InitListView));


            InitListView();
        }
        void InitListView()
        { 
            var titleView = FindViewById<TextView>(Resource.Id.FolderSelector_Title);
            var listView = FindViewById<RecyclerView>(Resource.Id.FolderSelector_RecyclerView);
            var Adapter = new ExplerAdapter(this, Environment.ExternalStorageDirectory.Path);
            Adapter.AfterChanged += item => {
                titleView.Text = item == null ? "未知" : (new DirectoryInfo(item.Parent)).Name;
            };
            listView.SetLayoutManager(new LinearLayoutManager(this));
            listView.SetAdapter(Adapter);
        }
    }
}