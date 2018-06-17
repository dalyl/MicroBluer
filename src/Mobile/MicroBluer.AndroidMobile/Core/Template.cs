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
using MicroBluer.AndroidMobile.Models;
using MicroBluer.AndroidMobile.Views;
using static Android.Resource;

namespace MicroBluer.AndroidMobile
{
    public class Template
    {
        public static string Layout(Setting setting)
        {
            var template = new LayoutView() { Model = setting  };
            return template.GenerateString();
        }

        public class Setting
        {
            public string Background { get; set; }
        }
    }
}