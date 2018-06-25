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
    using Android.Graphics;
    using Android.Media;
    using Android.Provider;

    public class ExplerAdapter : RecyclerView.Adapter
    {
        private Context Context { get; }

        public ExplerItemCollection Items { get; } = new ExplerItemCollection();

        public string CurrentRoot { get; private set; }

        public event Action AfterItemsChanged;

        public event Action<ExplerItem> ItemClick;

        public int SelectedPosition { get; set; } = -1;

        public ExplerAdapter(Context context) : base()
        {
            this.Context = context;
        }

        public override int ItemCount => Items.Count;

        public void SetData(string path)
        {
            TryCatch.Current.Invoke(() => {
                CurrentRoot = path;
                Items.Add(path);
                AfterItemsChanged?.Invoke();
            });
            NotifyDataSetChanged();
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

                if (item.IsPicture)
                {
                    var thumbnail = GetImageThumbnail(item.FullName, Icon.Width, Icon.Height);
                    Icon.SetImageBitmap(thumbnail);
                }
                else
                {
                    Icon.SetImageResource(item.Icon);
                }

                Size.Text = item.Size.FormetFileSize();
                Layout.SetOnLongClickListener(new AnonymousLongClickListener(v=> {
                    adapter.SelectedPosition = position;
                    return false;
                }));
            }

        }

        /**
          * @param imagePath
          *   图像的路径
          * @param width
          *   指定输出图像的宽度
          * @param height
          *   指定输出图像的高度
          * @return 生成的缩略图
          */
        public static Bitmap GetImageThumbnail(string imagePath, int width, int height)
        {
            Bitmap bitmap = null;
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            // 获取这个图片的宽和高，注意此处的bitmap为null
            bitmap = BitmapFactory.DecodeFile(imagePath, options);
            options.InJustDecodeBounds = false; // 设为 false
                                                // 计算缩放比
            int h = options.OutHeight;
            int w = options.OutWidth;
            int beWidth = w / width;
            int beHeight = h / height;
            int be = 1;
            if (beWidth < beHeight)
            {
                be = beWidth;
            }
            else
            {
                be = beHeight;
            }
            if (be <= 0)
            {
                be = 1;
            }
            options.InSampleSize = be;
            // 重新读入图片，读取缩放后的bitmap，注意这次要把options.inJustDecodeBounds 设为 false
            bitmap = BitmapFactory.DecodeFile(imagePath, options);
            // 利用ThumbnailUtils来创建缩略图，这里要指定要缩放哪个Bitmap对象
            bitmap = ThumbnailUtils.ExtractThumbnail(bitmap, width, height, ThumnailExtractOptions.RecycleInput);
            return bitmap;
        }

        /**
         * 获取视频的缩略图 先通过ThumbnailUtils来创建一个视频的缩略图，然后再利用ThumbnailUtils来生成指定大小的缩略图。
         * 如果想要的缩略图的宽和高都小于MICRO_KIND，则类型要使用MICRO_KIND作为kind的值，这样会节省内存。
         * 
         * @param videoPath
         *   视频的路径
         * @param width
         *   指定输出视频缩略图的宽度
         * @param height
         *   指定输出视频缩略图的高度度
         * @param kind
         *   参照MediaStore.Images.Thumbnails类中的常量MINI_KIND和MICRO_KIND。
         *   其中，MINI_KIND: 512 x 384，MICRO_KIND: 96 x 96
         * @return 指定大小的视频缩略图
         */
        public static Bitmap GetVideoThumbnail(String videoPath, int width, int height, ThumbnailKind kind)
        {
            Bitmap bitmap = null;
            bitmap = ThumbnailUtils.CreateVideoThumbnail(videoPath, kind);
            bitmap = ThumbnailUtils.ExtractThumbnail(bitmap, width, height, ThumnailExtractOptions.RecycleInput);
            return bitmap;
        }

    }
}