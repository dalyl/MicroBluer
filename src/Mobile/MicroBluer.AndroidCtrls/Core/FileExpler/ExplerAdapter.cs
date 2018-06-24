namespace MicroBluer.AndroidCtrls.FileExpler
{
    using System;
    using System.IO;
    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using Android.Support.V7.Widget;
    using MicroBluer.AndroidUtils.Views;
    using MicroBluer.AndroidUtils;
    using Resource = MicroBluer.AndroidCtrls.Resource;
    using MicroBluer.AndroidUtils.IO;

    public class ExplerAdapter : RecyclerView.Adapter
    {
        private Context Context { get; }

        private ExplerItemCollection Items { get; } = new ExplerItemCollection();

        public string CurrentRoot { get; private set; }

        public event Action AfterItemsChanged;

        public int SelectedPosition { get; set; } = -1;

        public ExplerAdapter(Context context) : base()
        {
            this.Context = context;
        }

        public override int ItemCount => Items.Count;

        public void SwitchContextMenu(IMenuItem item)
        {
            if (SelectedPosition == -1) return;
            if (Items.Count < SelectedPosition) return;
            var selected = Items[SelectedPosition];
            if (selected == null) return;
            SelectedPosition = -1;

            if (item.ItemId == Resource.Id.FileExplerorItem_Menu_Copy)
            {
                ClipboardManager manager = (ClipboardManager)Context.GetSystemService(Context.ClipboardService);
                manager.Text = selected.FullName;
                TryCatch.Current.Show($"{selected.Name}的路径已复制到剪切板");
                return;
            }

            if (item.ItemId == Resource.Id.FileExplerorItem_Menu_Rename)
            {
                var edit = new EditText(Context)
                {
                    Text = selected.Name
                };
                var editDialog = new AlertDialog.Builder(Context);
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

        void RenameClick(ExplerItem item,string text)
        {
            var last = item.FullName.LastIndexOf(item.Name);
            var path = item.FullName.Substring(0, last);
            var newPath =$"{path}{text}";
            TryCatch.Current.Invoke(() => File.Move(item.FullName, newPath));
        }

        public void SetData(string path)
        {
            TryCatch.Current.Invoke(() => {
                CurrentRoot = path;
                Items.Add(path);
                AfterItemsChanged?.Invoke();
            });
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
                TryCatch.Current.Show($"{item.Name}不是文件夹");
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ItemViewHolder)
            {
                var item = Items[position];
                var itemView = holder as ItemViewHolder;
                itemView.BindData(this, position, item);
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
            public TextView Size;

            public ItemViewHolder(View view) : base(view)
            {
                Icon = view.FindViewById<ImageView>(Resource.Id.FileExplerorItem_Icon);
                Path = view.FindViewById<TextView>(Resource.Id.FileExplerorItem_Path);
                Layout = view.FindViewById<RelativeLayout>(Resource.Id.FileExplerorItem_Layout);
                Size = view.FindViewById<TextView>(Resource.Id.FileExplerorItem_Size);
            }

            public void BindData(ExplerAdapter adapter,int position, ExplerItem item)
            {
                Path.Text = item.Name;
                Layout.SetOnClickListener(new AnonymousOnClickListener(v => adapter.ItemClick(item)));
                Path.SetOnClickListener(new AnonymousOnClickListener(v => adapter.ItemClick(item)));
                Icon.SetImageResource(item.Icon);
                Size.Text = item.Size.FormetFileSize();
                Layout.SetOnLongClickListener(new AnonymousLongClickListener(v=> {
                    adapter.SelectedPosition = position;
                    return false;
                }));
            }

        }

      
    }
}