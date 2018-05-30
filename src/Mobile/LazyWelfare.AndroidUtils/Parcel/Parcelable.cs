namespace LazyWelfare.AndroidUtils.Parcel
{
    using Object = Java.Lang.Object;
    using Parcel = Android.OS.Parcel;
    using Android.OS;
    using Android.Runtime;

    public abstract class Parcelable : Object, IParcelable
    {
        public abstract void ConvertFromParcel(Parcel data);

        public virtual int DescribeContents()
        {
            return 0;
        }

        public virtual void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {

        }
    }
}