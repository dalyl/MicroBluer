﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile
{
    public class Appcation
    {
        public static Context CurrentContext { get; set; }

        public static Action<bool, string> ScanResultDo { get; set; }
    }

}