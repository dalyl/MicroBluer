namespace MicroBluer.AndroidMobile
{

    using System.Collections.Generic;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using Object = Java.Lang.Object;

    public class MenuContentAdapter : BaseAdapter
    {

        private  Context context { get; }
        private IList<MenuContentItem> list { get; }

        public MenuContentAdapter(Context context, IList<MenuContentItem> list) : base()
        {
            this.context = context;
            this.list = list;
        }

        public override int Count
        {
            get
            {
                if (list != null)
                {
                    return list.Count;
                }
                return 0;
            }
        }

        public override Object GetItem(int position)
        {
            if (list != null)
            {
                return list[position];
            }
            return null;
        }

        public override long GetItemId(int position)
        {
            return list[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHold hold;
            if (convertView == null)
            {
                hold = new ViewHold();
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.MenuContent_item, null);
                convertView.Tag = hold;
            }
            else
            {
                hold = convertView.Tag as ViewHold;
            }

            hold.imageView = convertView.FindViewById<ImageView>(Resource.Id.item_imageview);
            hold.textView = convertView.FindViewById<TextView>(Resource.Id.item_textview);

            hold.imageView.SetImageResource(list[position].ImageView);
            hold.textView.Text = list[position].Text;
            return convertView;
        }

        internal class ViewHold : Object
        {
            public ImageView imageView;
            public TextView textView;
        }

    }

}