using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile.Utils
{
    public class ParcelUtils
    {
        public static bool ReadBoolean(Parcel source)
        {
            return source.ReadInt() == 1;
        }

        public static void WriteBoolean(Parcel dest, bool boolean )
        {
            dest.WriteInt(boolean ? 1 : 0);
        }


    }
}