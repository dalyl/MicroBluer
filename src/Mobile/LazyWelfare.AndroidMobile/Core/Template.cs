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
using LazyWelfare.AndroidMobile.Models;
using LazyWelfare.AndroidMobile.Views;
using static Android.Resource;

namespace LazyWelfare.AndroidMobile
{
    public class Template
    {
        public static string Layout()
        {
            var model = new AppModel
            {
                Header = "Hello,你好",
            };
            var template = new LayoutView() { Model = model };
            return template.GenerateString();
        }
    }
}