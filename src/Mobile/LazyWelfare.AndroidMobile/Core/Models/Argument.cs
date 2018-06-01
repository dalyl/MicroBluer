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
using LazyWelfare.Interface;

namespace LazyWelfare.AndroidMobile.Models
{
    public class Argument : IArgument
    {
        public string Service { get; set; }

        public string Uri{ get; set; }
}
}