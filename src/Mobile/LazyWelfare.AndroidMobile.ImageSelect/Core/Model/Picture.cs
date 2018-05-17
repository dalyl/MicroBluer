using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using LazyWelfare.AndroidMobile.ImageSelect.Utils;
using LazyWelfare.AndroidMobile.ImageSelect.Engine;
using Android.Provider;
using Android.Net;
using Android.Graphics;

namespace LazyWelfare.AndroidMobile.ImageSelect.Model
{

    using Cursor = Android.Database.ICursor;

    public class Picture :Java.Lang.Object , IParcelable
	{

		public const long ITEM_ID_CAPTURE = -1;
		public const string ITEM_DISPLAY_NAME_CAPTURE = "Capture";
		private readonly long mId;
		private string mDisplayName;

        internal Picture(long id, string displayName)
        {
            mId = id;
            mDisplayName = displayName;
        }

        internal Picture(Parcel source)
        {
            mId = source.ReadLong();
        }

		public static Picture ValueOf(Cursor cursor)
		{
	//        return new Picture(cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media._ID)));
			return new Picture(cursor.GetLong(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Id)),cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.DisplayName)));
		}

		public  int DescribeContents()
		{
			return 0;
		}

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteLong(mId);
        }

		public virtual long Id
		{
			get
			{
				return mId;
			}
		}

		public virtual Uri BuildContentUri()
		{
			return ContentUris.WithAppendedId(MediaStore.Images.Media.ExternalContentUri, mId);
		}

		public virtual Bitmap GetThumbnail(Context context)
		{
			BitmapFactory.Options options = new BitmapFactory.Options();
			options.InDither = false;
			options.InPreferredConfig = Bitmap.Config.Argb8888;
			ContentResolver contentResolver = context.ContentResolver;
			return MediaStore.Images.Thumbnails.GetThumbnail(contentResolver, mId, ThumbnailKind.MicroKind, options);
		}

		public virtual bool Capture
		{
			get
			{
				return mId == ITEM_ID_CAPTURE;
			}
		}
        public class PictureParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new Picture(source);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Object[size];
            }
        }


        [Java.Interop.ExportField("CREATOR")]
        public static PictureParcelableCreator Creator() => new PictureParcelableCreator();
    }
}