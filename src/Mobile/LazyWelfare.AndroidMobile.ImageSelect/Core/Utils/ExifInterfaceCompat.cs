using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Text;
using Java.Util;

namespace LazyWelfare.AndroidMobile.ImageSelect.Utils
{
    public class ExifInterfaceCompat
    {
        public static string TAG = nameof(ExifInterfaceCompat);

        private ExifInterfaceCompat()
        {
        }

        public static ExifInterface newInstance(string filename)
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

        public static Date getExifDateTime(string filepath)
        {
            ExifInterface exif;

            try
            {
                exif = newInstance(filepath);
            }
            catch (IOException var5)
            {
                //  Log.e(TAG, "cannot read exif", var5);
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
                    var e = new SimpleDateFormat("yyyy:MM:dd HH:mm:ss");
                    e.TimeZone = TimeZone.GetTimeZone("UTC");
                    return e.Parse(date);
                }
                catch (ParseException var4)
                {
                    // Log.d(TAG, "failed to parse date taken", var4);
                    return null;
                }
            }
        }

        public static long getExifDateTimeInMillis(string filepath)
        {
            Date datetime = getExifDateTime(filepath);
            return datetime == null ? -1L : datetime.Time;
        }

        public static int GetExifOrientation(string filepath)
        {
            ExifInterface exif = null;

            try
            {
                exif = newInstance(filepath);
            }
            catch (IOException var3)
            {
                // Log.e(TAG, "cannot read exif", var3);
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