using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using LazyWelfare.AndroidMobile.ImageSelect.Model;
using Android.Net;
using Java.Lang;
using Android.Content;

namespace LazyWelfare.AndroidMobile.ImageSelect.Loader
{
    public class AlbumLoader : Android.Support.V4.Content.CursorLoader
    {
        private static readonly string[] PROJECTION = new string[] { MediaStore.Images.Media.InterfaceConsts.BucketId, MediaStore.Images.Media.InterfaceConsts.BucketDisplayName, MediaStore.Images.Media.InterfaceConsts.Id, "count(bucket_id) as cou" };
        private const string BUCKET_GROUP_BY = ") GROUP BY  1,(2";
        private const string BUCKET_ORDER_BY = "MAX(datetaken) DESC";
        private static readonly String MEDIA_ID_DUMMY = new String ("-1");
        private const string IS_LARGE_SIZE = " _size > ? or _size is null";

        public static Android.Support.V4.Content.CursorLoader NewInstance(Context context, SelectionSpec selectionSpec)
        {
            return new AlbumLoader(context, MediaStore.Images.Media.ExternalContentUri, PROJECTION, IS_LARGE_SIZE + BUCKET_GROUP_BY, new string[] { selectionSpec.MinPixels + "" }, BUCKET_ORDER_BY, selectionSpec);
        }

        private AlbumLoader(Context context, Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder, SelectionSpec selectionSpec) : base(context, uri, projection, selection, selectionArgs, sortOrder)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>ICursor</returns>
        public override Java.Lang.Object LoadInBackground()
        {
            ICursor albums = base.LoadInBackground() as ICursor;
            MatrixCursor allAlbum = new MatrixCursor(PROJECTION);

            long count = 0;
            if (albums.Count > 0)
            {
                while (albums.MoveToNext())
                {
                    count += albums.GetLong(3);
                }
            }
            allAlbum.AddRow(new Object[] { Album.ALBUM_ID_ALL, Album.ALBUM_NAME_ALL, MEDIA_ID_DUMMY, new String($"{count}") });

            return new MergeCursor(new ICursor[] { allAlbum, albums });
        }
    }
}