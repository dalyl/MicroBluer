namespace LazyWelfare.AndroidCtrls.FileExpler
{
    using System;
    using System.Linq;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using Android.Support.V7.Widget;
    using LazyWelfare.AndroidUtils.Views;

    public  class ExplerAdapter : RecyclerView.Adapter
    {
        private Context Context { get; }

        private ExplerItemCollection Items { get; } = new ExplerItemCollection();

        public string CurrentRoot { get; private set; }

        public event Action AfterItemsChanged;

        public ExplerAdapter(Context context) : base()
        {
            this.Context = context;
        }

        public override int ItemCount => Items.Count;

        public void SetData(string path)
        {
            try
            {
                CurrentRoot = path;
                Items.Add(path);
                AfterItemsChanged?.Invoke();
            }
            catch (UnauthorizedAccessException ex)
            {
                Toast.MakeText(Context, "路径无权访问", ToastLength.Short).Show();
                return;
            }
            NotifyDataSetChanged();
        }

        public void ItemClick(ExplerItem item)
        {
            if (item.IsDirectory)
            {
                SetData(item.FullName);
            }
            else
            {
                Toast.MakeText(Context, $"{item.Name}不是文件夹", ToastLength.Short).Show();
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ItemViewHolder)
            {
                var item = Items[position];
                var itemView = holder as ItemViewHolder;
                itemView.Path.Text = item.Name;
                itemView.Layout.SetOnClickListener(new AnonymousOnClickListener(v => ItemClick(item)));
                itemView.Path.SetOnClickListener(new AnonymousOnClickListener(v => ItemClick(item)));
                itemView.Icon.SetImageResource(item.Icon);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(Context).Inflate(Resource.Layout.FileExplerorItem, parent, false);
            return new ItemViewHolder(view);
        }


        internal class ItemViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Icon;
            public TextView Path;
            public RelativeLayout Layout;
            public ImageView Menu;

            public ItemViewHolder(View view) : base(view)
            {
                Icon = view.FindViewById<ImageView>(Resource.Id.FileExplerorItem_Icon);
                Path = view.FindViewById<TextView>(Resource.Id.FileExplerorItem_Path);
                Layout = view.FindViewById<RelativeLayout>(Resource.Id.FileExplerorItem_Layout);
                Menu = view.FindViewById<ImageView>(Resource.Id.FileExplerorItem_Menu);
            }
        }

      
    }
}