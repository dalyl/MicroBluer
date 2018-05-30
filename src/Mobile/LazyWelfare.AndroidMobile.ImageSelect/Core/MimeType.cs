namespace LazyWelfare.AndroidMobile.ImageSelect
{
    using Parcelable = AndroidUtils.Parcel.Parcelable;
    using System.Collections.Generic;
    using Android.Content;
    using Android.Net;
    using Android.OS;
    using Android.Runtime;
    using Android.Webkit;
    using Java.Lang;
    using LazyWelfare.AndroidMobile.ImageSelect.Utils;
    using LazyWelfare.AndroidUtils.Parcel;

    public sealed class MimeType : Parcelable
    {

        public static MimeType JPEG = new MimeType("image/jpeg", new string[] { "jpg", "jpeg" });
        public static MimeType PNG = new MimeType("image/png", new string[] { "png" });
        public static MimeType GIF = new MimeType("image/gif", new string[] { "gif" });


        private string mMimeTypeName;

        private List<string> mExtensions=new List<string>();

        public MimeType(string mimeTypeName, IEnumerable<string> extensions)
        {
            mMimeTypeName = mimeTypeName;
            mExtensions.AddRange(extensions);
        }
        public MimeType(){ }

        internal MimeType(Parcel data)
        {
            ConvertFromParcel(data);
        }
        public override void ConvertFromParcel(Parcel data)
        {
            mMimeTypeName = data.ReadString();
            data.ReadStringList(mExtensions);
        }

        public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteString(mMimeTypeName);
            dest.WriteStringList(mExtensions);
        }


        public static List<MimeType> AllOf()
        {
            var values = new List<MimeType>() { JPEG, PNG , GIF };
            return values;
        }

        public new string ToString()
        {
            return mMimeTypeName;
        }

        public bool CheckType(ContentResolver resolver, Uri uri)
        {
            MimeTypeMap map = MimeTypeMap.Singleton;
            if (uri == null)
            {
                return false;
            }
            string type = map.GetExtensionFromMimeType(resolver.GetType(uri));
            foreach (string extension in mExtensions)
            {
                if (extension.Equals(type))
                {
                    return true;
                }
                string path = PhotoMetadataUtils.GetPath(resolver, uri);
                if (path != null && path.ToLower().EndsWith(extension))
                {
                    return true;
                }
            }
            return false;
        }

        [Java.Interop.ExportField("CREATOR")]
        public static ParcelableCreator<MimeType> Creator() => new ParcelableCreator<MimeType>();

       
    }
}

  

