namespace MicroBluer.AndroidCtrls.ImageExpleror.Loader
{
    using CursorLoader = Android.Support.V4.Content.CursorLoader;
    using Object = Java.Lang.Object;
    using Android.Database;
    using Android.Provider;
    using MicroBluer.AndroidCtrls.ImageExpleror.Model;
    using Android.Net;
    using Android.Content;

    public class AlbumLoader : CursorLoader
    {
        private static readonly string[] PROJECTION = new string[] { MediaStore.Images.Media.InterfaceConsts.BucketId, MediaStore.Images.Media.InterfaceConsts.BucketDisplayName, MediaStore.Images.Media.InterfaceConsts.Id, "count(bucket_id) as cou" };
        private const string BUCKET_GROUP_BY = ") GROUP BY  1,(2";
        private const string BUCKET_ORDER_BY = "MAX(datetaken) DESC";
        private const string MEDIA_ID_DUMMY = "-1";
        private const string IS_LARGE_SIZE = " _size > ? or _size is null";

        public static CursorLoader NewInstance(Context context, SelectionSpec selectionSpec)
        {
            return new AlbumLoader(context, MediaStore.Images.Media.ExternalContentUri, PROJECTION, IS_LARGE_SIZE + BUCKET_GROUP_BY, new string[] { selectionSpec.MinPixels + "" }, BUCKET_ORDER_BY, selectionSpec);
        }

        private AlbumLoader(Context context, Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder, SelectionSpec selectionSpec) : base(context, uri, projection, selection, selectionArgs, sortOrder)
        {
        }

        public override Object LoadInBackground()
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
            allAlbum.AddRow(new Object[] { Album.ALBUM_ID_ALL, Album.ALBUM_NAME_ALL, MEDIA_ID_DUMMY, $"{count}" });

            return new MergeCursor(new ICursor[] { allAlbum, albums });
        }
    }
}