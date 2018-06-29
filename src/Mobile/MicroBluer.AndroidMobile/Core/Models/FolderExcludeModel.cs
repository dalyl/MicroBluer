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

namespace MicroBluer.AndroidMobile.Models
{

  

    public class FolderExcludeModel
    {
        public Guid Guid { get; set; }

        public string Path { get; set; }
        public FolderKind Type { get; set; }
    }
}