

namespace LazyWelfare.AndroidCtrls.ImageExpleror.Model
{
    using Parcelable = AndroidUtils.Parcel.Parcelable;
    using Cursor = Android.Database.ICursor;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Provider;
    using Android.Net;
    using Android.Graphics;
    using LazyWelfare.AndroidUtils.Parcel;

    public class Picture : Parcelable
    {
		public const long ITEM_ID_CAPTURE = -1;
		public const string ITEM_DISPLAY_NAME_CAPTURE = "Capture";

        public virtual long Id { get; private set; }

        private string mDisplayName;

        public Picture() { }

        internal Picture(long id, string displayName)
        {
            Id = id;
            mDisplayName = displayName;
        }

        internal Picture(Parcel data)
        {
            ConvertFromParcel(data);
        }

        public override void ConvertFromParcel(Parcel data)
        {
            Id = data.ReadLong();
        }

        public static Picture ValueOf(Cursor cursor)
		{
	//        return new Picture(cursor.getLong(cursor.getColumnIndex(MediaStore.Images.Media._ID)));
			return new Picture(cursor.GetLong(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Id)),cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.DisplayName)));
		}

        public  override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteLong(Id);
        }

		public virtual Uri BuildContentUri()
		{
			return ContentUris.WithAppendedId(MediaStore.Images.Media.ExternalContentUri, Id);
		}

		public virtual Bitmap GetThumbnail(Context context)
		{
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InDither = false,
                InPreferredConfig = Bitmap.Config.Argb8888
            };
            ContentResolver contentResolver = context.ContentResolver;
			return MediaStore.Images.Thumbnails.GetThumbnail(contentResolver, Id, ThumbnailKind.MicroKind, options);
		}

		public virtual bool Capture
		{
			get
			{
				return Id == ITEM_ID_CAPTURE;
			}
		}

        [Java.Interop.ExportField("CREATOR")]
        public static ParcelableCreator<Picture> Creator() => new ParcelableCreator<Picture>();
       
    }
}