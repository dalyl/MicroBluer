namespace MicroBluer.AndroidUtils.Parcel
{
    using Parcel = Android.OS.Parcel;
    using Object = Java.Lang.Object;
    using Android.OS;

    public class ParcelableCreator<T> : Object, IParcelableCreator where T : Parcelable, new()
    {
        public Object CreateFromParcel(Parcel data)
        {
            var model = new T();
            model.ConvertFromParcel(data);
            return model;
        }

        public Object[] NewArray(int size)
        {
            return new Object[size];
        }
    }

}