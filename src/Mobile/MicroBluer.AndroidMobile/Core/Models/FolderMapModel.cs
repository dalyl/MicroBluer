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
    public class FolderMapModel
    {

        public Guid Guid { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 源文件夹
        /// </summary>
        public string MapFolder { get; set; }

        /// <summary>
        /// 归档文件夹
        /// </summary>
        public string InnerFolder { get; set; }

    }
}