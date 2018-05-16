using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Java.Lang;

namespace LazyWelfare.AndroidMobile.ImageSelect.Model
{

    public class Album  : Object, IParcelable 
    {

		public static readonly string ALBUM_ID_ALL = "-1";
		public static string ALBUM_NAME_ALL = "All";
		public const string ALBUM_NAME_CAMERA = "Camera";
		public const string ALBUM_NAME_DOWNLOAD = "Download";
		public const string ALBUM_NAME_SCREEN_SHOT = "Screenshots";
		private readonly string mId; //BUCKET_ID
		private readonly long mCoverId; //Media_ID
		private readonly string mDisplayName; //BUCKET_DISPLAY_NAME
		private readonly string mCount; // photo count

		/* package */

		public Album(string id, long coverId, string albumName, string count)
		{
			mId = id;
			mCoverId = coverId;
			mDisplayName = albumName;
			mCount = count;
		}

        /* package */
        internal Album(Parcel source) 
        {
			mId = source.ReadString();
			mCoverId = source.ReadLong();
			mDisplayName = source.ReadString();
			mCount = source.ReadString();
	 }

		/// <summary>
		/// This method is not responsible for managing cursor resource, such as close, iterate, and so on.
		/// </summary>
		/// <param name="cursor"> to be converted. </param>
		public static Album ValueOf(ICursor cursor)
		{
			return new Album(cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.BucketId)), cursor.GetLong(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Id)), cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.BucketDisplayName)), cursor.GetLong(3) + "");
		}

		public  int DescribeContents()
		{
			return 0;
		}

		public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
		{
			dest.WriteString(mId);
			dest.WriteLong(mCoverId);
			dest.WriteString(mDisplayName);
			dest.WriteString(mCount);
		}

		public virtual string Id
		{
			get
			{
				return mId;
			}
		}

		public virtual long CoverId
		{
			get
			{
				return mCoverId;
			}
		}

		public virtual string GetDisplayName(Context context)
		{
			if (All)
			{
				return context.GetString(Resource.String.l_album_name_all);
			}
			if (Camera)
			{
				return context.GetString(Resource.String.l_album_name_camera);
			}
			if (ALBUM_NAME_DOWNLOAD.Equals(mDisplayName))
			{
				return context.GetString(Resource.String.l_album_name_download);
			}
			if (ALBUM_NAME_SCREEN_SHOT.Equals(mDisplayName))
			{
				return context.GetString(Resource.String.l_album_name_screen_shot);
			}
			return mDisplayName;
		}

        

        public virtual Uri BuildContentUri()
		{
			return ContentUris.WithAppendedId(MediaStore.Images.Media.ExternalContentUri, mCoverId);
		}

		public virtual bool All
		{
			get
			{
				return ALBUM_ID_ALL.Equals(mId);
			}
		}

		public virtual bool Camera
		{
			get
			{
				return ALBUM_NAME_CAMERA.Equals(mDisplayName);
			}
		}

		public virtual string Count
		{
			get
			{
				return mCount;
			}
		}

        public class AlbumParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            public Java.Lang.Object CreateFromParcel(Parcel source)
            {
                return new Album(source);
            }

            public Java.Lang.Object[] NewArray(int size)
            {
                return new Object[size];
            }
        }


        [Java.Interop.ExportField("CREATOR")]
        public static AlbumParcelableCreator Creator() => new AlbumParcelableCreator();
    }

}