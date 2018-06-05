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

namespace LazyWelfare.AndroidMobile.Models
{
    public class FolderMapModel
    {

        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string MapFolder { get; set; }

    }
}