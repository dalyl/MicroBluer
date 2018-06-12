using Java.IO;
using Java.Lang;
using Java.Nio.Channels;
using Java.Text;
using Java.Util;
using LazyWelfare.AndroidUtils.Database;

namespace LazyWelfare.AndroidCtrls.ImageExpleror.Utils
{

    using Activity = Android.App.Activity;
    using ContentResolver = Android.Content.ContentResolver;
    using ContentUris = Android.Content.ContentUris;
    using ContentValues = Android.Content.ContentValues;
    using Context = Android.Content.Context;
    using Intent = Android.Content.Intent;
    using PackageManager = Android.Content.PM.PackageManager;
    using ContentObserver = Android.Database.ContentObserver;
    using Cursor = Android.Database.ICursor;
    using Options = Android.Graphics.BitmapFactory.Options;
    using Uri = Android.Net.Uri;
    using Environment = Android.OS.Environment;
    using Handler = Android.OS.Handler;
    using Media = Android.Provider.MediaStore.Images.Media;
    using Thumbnails = Android.Provider.MediaStore.Images.Thumbnails;


    public class MediaStoreCompat
    {
        public static readonly string TAG = typeof(MediaStoreCompat).Name;
        private const string MEDIA_FILE_NAME_FORMAT = "yyyyMMdd_HHmmss";
        private const string MEDIA_FILE_EXTENSION = ".jpg";
        private const string MEDIA_FILE_PREFIX = "IMG_";
        private readonly string MEDIA_FILE_DIRECTORY;
        private Context mContext;
        private ContentObserver mObserver;
        private System.Collections.Generic.List<PhotoContent> mRecentlyUpdatedPhotos;

        public MediaStoreCompat(Context context, Handler handler)
        {
            this.mContext = context;
            this.MEDIA_FILE_DIRECTORY = context.PackageName;
            this.mObserver = new AnonymousContentObserver(handler, state => this.UpdateLatestPhotos());
            this.mContext.ContentResolver.RegisterContentObserver(Media.ExternalContentUri, true, this.mObserver);
        }

        public static bool HasCameraFeature(Context context)
        {
            PackageManager pm = context.ApplicationContext.PackageManager;
            return pm.HasSystemFeature("android.hardware.camera");
        }

        public virtual string InvokeCameraCapture(Activity activity, int requestCode)
        {
            if (!HasCameraFeature(this.mContext))
            {
                return null;
            }
            else
            {
                File toSave = this.OutputFileUri;
                if (toSave == null)
                {
                    return null;
                }
                else
                {
                    Intent intent = new Intent("android.media.action.IMAGE_CAPTURE");
                    intent.AddCategory("android.intent.category.DEFAULT");
                    intent.PutExtra("output", Uri.FromFile(toSave));
                    intent.PutExtra("android.intent.extra.videoQuality", 1);
                    activity.StartActivityForResult(intent, requestCode);
                    return toSave.ToString();
                }
            }
        }

        public virtual void Destroy()
        {
            this.mContext.ContentResolver.UnregisterContentObserver(this.mObserver);
        }

        public virtual Uri GetCapturedPhotoUri(Intent data, string preparedUri)
        {
            Uri captured = null;
            if (data != null)
            {
                captured = data.Data;
                if (captured == null)
                {
                    data.GetParcelableExtra("android.intent.extra.STREAM");
                }
            }

            File prepared = new File(preparedUri.ToString());
            if (captured == null || captured.Equals(Uri.FromFile(prepared)))
            {
                captured = this.FindPhotoFromRecentlyTaken(prepared);
                if (captured == null)
                {
                    captured = this.StoreImage(prepared);
                    prepared.Delete();
                }
                else
                {
                    string realPath = GetPathFromUri(this.mContext.ContentResolver, captured);
                    if (!string.ReferenceEquals(realPath, null) && !prepared.Equals(new File(realPath)))
                    {
                        prepared.Delete();
                    }
                }
            }
            return captured;
        }

        public virtual void CleanUp(string uri)
        {
            File file = new File(uri.ToString());
            if (file.Exists())
            {
                file.Delete();
            }
        }

        public static string GetPathFromUri(ContentResolver resolver, Uri contentUri)
        {
            string dataColumn = "_data";
            Cursor cursor = null;

            string var5;
            try
            {
                cursor = resolver.Query(contentUri, new string[] { dataColumn }, (string)null, (string[])null, (string)null);
                if (cursor == null || !cursor.MoveToFirst())
                {
                    object index1 = null;
                    return (string)index1;
                }

                int index = cursor.GetColumnIndex("_data");
                var5 = cursor.GetString(index);
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }

            }

            return var5;
        }

        static void CopyFile(System.IO.FileStream fsRead, System.IO.FileStream fsWrite)
        {
            //创建一个负责读取的流
            byte[] buffer = new byte[1024 * 1024 * 5];

            //因为文件可能比较大所以在读取的时候应该用循坏去读取
            while (true)
            {
                //返回本次实际读取到的字节数
                int r = fsRead.Read(buffer, 0, buffer.Length);

                if (r == 0)
                {
                    break;
                }
                fsWrite.Write(buffer, 0, r);//写入
            }
        }

        static long CopyFileStream(FileInputStream @is, FileOutputStream os)
        {
            FileChannel srcChannel = null;
            FileChannel destChannel = null;
            long length;
            try
            {
                srcChannel = @is.Channel;
                destChannel = os.Channel;
                length = srcChannel.TransferTo(0L, srcChannel.Size(), destChannel);
            }
            finally
            {
                if (srcChannel != null)
                {
                    srcChannel.Close();
                }
                if (destChannel != null)
                {
                    destChannel.Close();
                }
            }
            return length;
        }

        private Uri FindPhotoFromRecentlyTaken(File file)
        {
            if (this.mRecentlyUpdatedPhotos == null)
            {
                this.UpdateLatestPhotos();
            }
            long fileSize = file.Length();
            long taken = ExifInterfaceCompat.getExifDateTimeInMillis(file.AbsolutePath);
            int maxPoint = 0;
            MediaStoreCompat.PhotoContent maxItem = null;

            foreach (var item in this.mRecentlyUpdatedPhotos)
            {
                int point = 0;
                if ((long)item.size == fileSize)
                {
                    ++point;
                }
                if (item.taken == taken)
                {
                    ++point;
                }
                if (point > maxPoint)
                {
                    maxPoint = point;
                    maxItem = item;
                }
            }

            if (maxItem != null)
            {
                this.GenerateThumbnails(maxItem.id);
                return ContentUris.WithAppendedId(Media.ExternalContentUri, maxItem.id);
            }
            else
            {
                return null;
            }
        }

        private Uri StoreImage(File file)
        {
            try
            {
                ContentValues e = new ContentValues();
                e.Put("title", file.Name);
                e.Put("mime_type", "image/jpeg");
                e.Put("description", "mixi Photo");
                e.Put("orientation", ExifInterfaceCompat.GetExifOrientation(file.AbsolutePath));
                long date = ExifInterfaceCompat.getExifDateTimeInMillis(file.AbsolutePath);
                if (date != -1L)
                {
                    e.Put("datetaken", date);
                }

                Uri imageUri = this.mContext.ContentResolver.Insert(Media.ExternalContentUri, e);
                using (System.IO.FileStream fos = (System.IO.FileStream)this.mContext.ContentResolver.OpenOutputStream(imageUri))
                {
                    using (System.IO.FileStream fis = new System.IO.FileStream(file.AbsolutePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        CopyFile(fos, fis);
                    }
                }
                this.GenerateThumbnails(ContentUris.ParseId(imageUri));
                return imageUri;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void UpdateLatestPhotos()
        {
            Cursor c = Media.Query(this.mContext.ContentResolver, Media.ExternalContentUri, new string[] { "_id", "datetaken", "_size" }, (string)null, (string[])null, "date_added DESC");
            if (c != null)
            {
                try
                {
                    int count = 0;
                    this.mRecentlyUpdatedPhotos = new System.Collections.Generic.List<PhotoContent>();

                    while (c.MoveToNext())
                    {
                        MediaStoreCompat.PhotoContent item = new MediaStoreCompat.PhotoContent
                        {
                            id = c.GetLong(0),
                            taken = c.GetLong(1),
                            size = c.GetInt(2)
                        };
                        this.mRecentlyUpdatedPhotos.Add(item);
                        ++count;
                        if (count > 5)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    c.Close();
                }
            }
        }

        private void GenerateThumbnails(long imageId)
        {
            try
            {
                Thumbnails.GetThumbnail(this.mContext.ContentResolver, imageId, Android.Provider.ThumbnailKind.MiniKind, (Options)null);
            }
            catch (System.NullReferenceException e)
            {
                //Console.WriteLine(e.ToString());
                //Console.Write(e.StackTrace);
            }
        }

        private File OutputFileUri
        {
            get
            {
                File extDir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), this.MEDIA_FILE_DIRECTORY);
                if (!extDir.Exists() && !extDir.Mkdirs())
                {
                    return null;
                }
                else
                {
                    string timeStamp = System.DateTime.Now.ToString(MEDIA_FILE_NAME_FORMAT);
                    return new File(extDir.Path + File.Separator + MEDIA_FILE_PREFIX + timeStamp + MEDIA_FILE_EXTENSION);
                }
            }
        }

        private class PhotoContent
        {
            public long id;
            public long taken;
            public int size;
        }
    }
}