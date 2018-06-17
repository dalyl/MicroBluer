namespace LazyWelfare.AndroidCtrls.ImageExpleror.Utils
{
    using Android.Media;
    using Android.Text;
    using Java.Lang;
    using Java.Text;
    using Java.Util;

    public class ExifInterfaceCompat
    {
        public static string TAG = nameof(ExifInterfaceCompat);

        private ExifInterfaceCompat()
        {
        }

        public static ExifInterface NewInstance(string filename)
        {
            if (filename == null)
            {
                throw new NullPointerException("filename should not be null");
            }
            else
            {
                return new ExifInterface(filename);
            }
        }

        public static Date GetExifDateTime(string filepath)
        {
            ExifInterface exif;

            try
            {
                exif = NewInstance(filepath);
            }
            catch //(IOException var5)
            {
                return null;
            }

            string date = exif.GetAttribute("DateTime");
            if (TextUtils.IsEmpty(date))
            {
                return null;
            }
            else
            {
                try
                {
                    var e = new SimpleDateFormat("yyyy:MM:dd HH:mm:ss")
                    {
                        TimeZone = TimeZone.GetTimeZone("UTC")
                    };
                    return e.Parse(date);
                }
                catch// (ParseException var4)
                {
                    return null;
                }
            }
        }

        public static long GetExifDateTimeInMillis(string filepath)
        {
            Date datetime = GetExifDateTime(filepath);
            return datetime == null ? -1L : datetime.Time;
        }

        public static int GetExifOrientation(string filepath)
        {
            ExifInterface exif = null;

            try
            {
                exif = NewInstance(filepath);
            }
            catch //(IOException var3)
            {
                return -1;
            }

            int orientation = exif.GetAttributeInt("Orientation", -1);
            if (orientation == -1)
            {
                return 0;
            }
            else
            {
                switch (orientation)
                {
                    case 3:
                        return 180;
                    case 6:
                        return 90;
                    case 8:
                        return 270;
                    default:
                        return 0;
                }
            }
        }
    }
}