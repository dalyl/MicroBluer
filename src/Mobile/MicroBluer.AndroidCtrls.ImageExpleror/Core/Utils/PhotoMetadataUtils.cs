using Android.Content;
using Android.Net;
using Android.Provider;


namespace MicroBluer.AndroidCtrls.ImageExpleror.Utils
{
    public class PhotoMetadataUtils
    {
        private const string SCHEME_CONTENT = "content";

        public static string GetPath(ContentResolver resolver, Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            if (SCHEME_CONTENT.Equals(uri.Scheme))
            {
                //Cursor cursor = null;
                //try
                //{
                //    cursor = resolver.Query(uri, new string[] { MediaStore.Images.ImageColumns.Data }, null, null, null);
                //    if (cursor == null || !cursor.moveToFirst())
                //    {
                //        return null;
                //    }
                //    return cursor.getString(cursor.getColumnIndex(MediaStore.Images.ImageColumns.Data));
                //}
                //finally
                //{
                //    if (cursor != null)
                //    {
                //        cursor.close();
                //    }
                //}


                var cursor = resolver.Query(uri, new string[] { MediaStore.Images.ImageColumns.Data }, null, null, null);
                try
                {
                    if (cursor == null || !cursor.MoveToFirst())
                    {
                        return null;
                    }
                    return cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data));
                }
                finally
                {
                    if (cursor != null)
                    {
                        cursor.Close();
                    }
                }
            }
            return uri.Path;
        }

    }
}