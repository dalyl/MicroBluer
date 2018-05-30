namespace LazyWelfare.AndroidMobile.ImageSelect.Adapter
{
    using Context = Android.Content.Context;
    using Cursor = Android.Database.ICursor;
    using CursorAdapter = Android.Support.V4.Widget.CursorAdapter;
    using LayoutInflater = Android.Views.LayoutInflater;
    using View = Android.Views.View;
    using ViewGroup = Android.Views.ViewGroup;
    using ImageView = Android.Widget.ImageView;
    using LazyWelfare.AndroidMobile.ImageSelect.Collection;
    using LazyWelfare.AndroidMobile.ImageSelect.Model;
    using LazyWelfare.AndroidMobile.ImageSelect.Widget;

    public class PictureAdapter : CursorAdapter
    {
        internal LayoutInflater mInflater;
        internal SelectedUriCollection mCollection;

        public PictureAdapter(Context context, Cursor c, SelectedUriCollection mCollection) : base(context, c, CursorAdapter.FlagRegisterContentObserver)
        {
            mInflater = LayoutInflater.From(context);
            this.mCollection = mCollection;
        }

        private ViewHolder viewHolder;

        public override View NewView(Context context, Cursor cursor, ViewGroup parent)
        {
            View convertView = mInflater.Inflate(Resource.Layout.ImageSelect_PhotoPickGridListItem, parent, false);
            viewHolder = new ViewHolder(convertView, mCollection);
            return convertView;
        }

        public override void BindView(View view, Context context, Cursor cursor)
        {
            viewHolder = view.Tag as ViewHolder;
            viewHolder.itemView.Item = Picture.ValueOf(cursor);
        }

        internal class ViewHolder:Java.Lang.Object
        {
            internal GridViewItemRelativeLayout itemView;

            public ViewHolder(View convertView, SelectedUriCollection mCollection)
            {
                itemView = (GridViewItemRelativeLayout)convertView;
                itemView.SetImageView((ImageView)convertView.FindViewById(Resource.Id.thumbnail), (ImageView)convertView.FindViewById(Resource.Id.check), mCollection);
                convertView.Tag = this;
            }
        }
    }
}