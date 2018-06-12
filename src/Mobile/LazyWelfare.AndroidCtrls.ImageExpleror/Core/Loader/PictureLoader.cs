namespace LazyWelfare.AndroidCtrls.ImageExpleror.Loader
{
    using Context = Android.Content.Context;
    using Cursor = Android.Database.ICursor;
    using MatrixCursor = Android.Database.MatrixCursor;
    using MergeCursor = Android.Database.MergeCursor;
    using Uri = Android.Net.Uri;
    using MediaStore = Android.Provider.MediaStore;
    using CursorLoader = Android.Support.V4.Content.CursorLoader;
    using LazyWelfare.AndroidCtrls.ImageExpleror.Model;
    using Java.Lang;
    using LazyWelfare.AndroidCtrls.ImageExpleror.Utils;

    public class PictureLoader : CursorLoader
    {
        private static readonly string[] PROJECTION = new string[] { MediaStore.Images.Media.InterfaceConsts.Id, MediaStore.Images.Media.InterfaceConsts.DisplayName };
        private static readonly string ORDER_BY = MediaStore.Images.Media.InterfaceConsts.Id + " DESC";
        private readonly bool mEnableCapture;
        private const string IS_LARGE_SIZE = "_size > ? or _size is null";

        public PictureLoader(Context context, Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder, bool capture) : base(context, uri, projection, selection, selectionArgs, sortOrder)
        {
            mEnableCapture = capture;
        }

        public static CursorLoader NewInstance(Context context, Album album, SelectionSpec selectionSpec)
        {
            if (album == null || album.All)
            {
                return new PictureLoader(context, MediaStore.Images.Media.ExternalContentUri, PROJECTION, IS_LARGE_SIZE, new string[] { selectionSpec.MinPixels + "" }, ORDER_BY, selectionSpec.IsEnableCamera);
            }
            return new PictureLoader(context, MediaStore.Images.Media.ExternalContentUri, PROJECTION, MediaStore.Images.Media.InterfaceConsts.BucketId + " = ? and (" + IS_LARGE_SIZE + ")", new string[] { album.Id, selectionSpec.MinPixels + "" }, ORDER_BY, selectionSpec.IsEnableCamera);
        }

        public override Object LoadInBackground()
        {
            var data = base.LoadInBackground();
            if (!mEnableCapture || !MediaStoreCompat.HasCameraFeature(Context))
            {
                return data;
            }

            var result = data as Cursor;
            MatrixCursor dummy = new MatrixCursor(PROJECTION);
            dummy.AddRow(new Object[] { Picture.ITEM_ID_CAPTURE, Picture.ITEM_DISPLAY_NAME_CAPTURE });
            return new MergeCursor(new Cursor[] { dummy, result });
        }
    }

}