namespace LazyWelfare.AndroidCtrls.FileSelect
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Android.Content;
    using Android.Support.V7.Widget;
    using Android.Views;
    using Android.Widget;
    using Object = Java.Lang.Object;
    using System.IO;
    using LazyWelfare.AndroidUtils.Views;

    public class SelectorAdapter : RecyclerView.Adapter
    {

        private Context Context { get; }

        private SelectorItemCollection Items { get; } = new SelectorItemCollection();

        private bool IsEmptyStyle { get;  set; } = true;

        private bool IsSelectMany { get; } = false;

        private SelectorType SelectorType { get; } =  SelectorType.Directory;

        public List<string> Selects { get; } = new List<string>();

        public event Action<SelectorItem> AfterChanged;

        public SelectorAdapter(Context context, SelectorType type, bool isSelectMany, string path) : base()
        {
            this.Context = context;
            SelectorType = type;
            IsSelectMany = isSelectMany;
            SetData(path);
        }

        public string CurrentRoot
        {
            get
            {
                if (Items.Count == 0) return string.Empty;
                return Items.First().Parent;
            }
        }

        public override int ItemCount => Items.Count;

        void CheckedChange(string path, bool isChecked)
        {
            if (IsSelectMany==false)
            {
                Selects.Clear();
                if (isChecked) Selects.Add(path);
                return;
            }

            if (isChecked)
            {
                if (Selects.Contains(path)) return;
                Selects.Add(path);
            }
            else
            {
                if (Selects.Contains(path) == false) return;
                Selects.Remove(path);
            }
        }

        public void SetData(string path)
        {
            try
            {
                IsEmptyStyle = true;
                this.Selects.Clear();
                Items.Add(path, SelectorType);
                if (Items.Count == 0)
                {
                    Items.Add(new SelectorItem { Parent= path });
                }
                else {
                    IsEmptyStyle = false;
                }
                AfterChanged?.Invoke(Items.FirstOrDefault());
            }
            catch (UnauthorizedAccessException ex)
            {
                Toast.MakeText(Context, "路径无权访问", ToastLength.Short).Show();
                return;
            }
            NotifyDataSetChanged();
        }

        public void ItemClick(SelectorItem item)
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
            if (holder is ItemViewHolder) {
                var item = Items[position];
                var itemView = holder as ItemViewHolder;
                itemView.CheckBox.CheckedChange += (sender, e) => CheckedChange(item.FullName, e.IsChecked);
                itemView.CheckBox.Checked = Selects.Contains(item.FullName);
                itemView.Path.Text = item.Name;
                itemView.Layout.SetOnClickListener(new AnonymousOnClickListener(v => ItemClick(item)));
                itemView.Path.SetOnClickListener(new AnonymousOnClickListener(v => ItemClick(item)));
                itemView.CheckBox.Visibility = item.IsSelectable ? ViewStates.Visible : ViewStates.Invisible;
                itemView.Icon.SetImageResource(item.Icon);
            }
            if (holder is EmptyItemViewHolder)
            {
               
            }
        }

        public override int GetItemViewType(int position)
        {
            if (IsEmptyStyle) return (int)SelectorItemType.Empty;
            return (int)SelectorItemType.Nomal;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == (int)SelectorItemType.Empty)
            {
                View empty = LayoutInflater.From(Context).Inflate(Resource.Layout.FileSelectorEmptyItem, parent, false);
                return new EmptyItemViewHolder(empty);
            }
            View view = LayoutInflater.From(Context).Inflate(Resource.Layout.FileSelectorItem, parent, false);
            return new ItemViewHolder(view);
        }
     

        internal class ItemViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Icon;
            public TextView Path;
            public RelativeLayout Layout;
            public CheckBox CheckBox;

            public ItemViewHolder(View view) : base(view)
            {
                Icon = view.FindViewById<ImageView>(Resource.Id.FolderSelectorItem_Image);
                Path = view.FindViewById<TextView>(Resource.Id.FolderSelectorItem_Path);
                Layout = view.FindViewById<RelativeLayout>(Resource.Id.FolderSelectorItem_Layout);
                CheckBox = view.FindViewById<CheckBox>(Resource.Id.FolderSelectorItem_CheckBox);
            }
        }

        internal class EmptyItemViewHolder : RecyclerView.ViewHolder
        {
            public TextView EmptyTip;
            public EmptyItemViewHolder(View view) : base(view)
            {
                EmptyTip = view.FindViewById<TextView>(Resource.Id.FolderSelectorEmptyItem_Tip);
            }
        }
    }
}