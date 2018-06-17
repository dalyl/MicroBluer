namespace MicroBluer.AndroidUtils.Extension
{
    using Android.OS;

    public static class ParcelExtension
    {
        public static bool ReadBoolean(this Parcel source)
        {
            return source.ReadInt() == 1;
        }

        public static void WriteBoolean(this Parcel dest, bool boolean)
        {
            dest.WriteInt(boolean ? 1 : 0);
        }


    }
}