using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LazyWelfare.AndroidMobile.ImageSelect.Model;

namespace LazyWelfare.AndroidMobile.ImageSelect.Adapter
{
    using Context = Android.Content.Context;
    using Cursor = Android.Database.ICursor;
    using CursorAdapter = Android.Support.V4.Widget.CursorAdapter;
    using Log = Android.Util.Log;
    using LayoutInflater = Android.Views.LayoutInflater;
    using View = Android.Views.View;
    using ViewGroup = Android.Views.ViewGroup;
    using ImageView = Android.Widget.ImageView;
    using TextView = Android.Widget.TextView;

    public class AlbumAdapter : CursorAdapter
    {
        internal LayoutInflater mInflater;
        internal ViewHolder viewHolder;

        public AlbumAdapter(Context context, Cursor c) : base(context, c, CursorAdapter.FlagRegisterContentObserver)
        {
            mInflater = LayoutInflater.From(context);
        }

        public override View NewView(Context context, Cursor cursor, ViewGroup parent)
        {
            View convertView = mInflater.Inflate(Resource.Layout.ImageSelect_PhotoPickListItem, parent, false);
            viewHolder = new ViewHolder();
            viewHolder.textView = (TextView)convertView.FindViewById(Resource.Id.foldName);
            viewHolder.photoCount = (TextView)convertView.FindViewById(Resource.Id.photoCount);
            convertView.Tag = viewHolder;
            return convertView;
        }

        public override void BindView(View view, Context context, Cursor cursor)
        {
            viewHolder = (ViewHolder)view.Tag;
            Album album = Album.ValueOf(cursor);
            viewHolder.textView.Text = album.GetDisplayName(context);
            viewHolder.photoCount.Text = "( " + album.Count + " )";
        }

        internal class ViewHolder:Object
        {
            internal TextView textView;
            internal TextView photoCount;
        }
    }
}