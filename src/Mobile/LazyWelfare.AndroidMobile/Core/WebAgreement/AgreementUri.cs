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

namespace LazyWelfare.AndroidMobile.WebAgreement
{
    public struct AgreementUri
    {
        public AgreementUri(string name, string host, string path)
        {
            Name = name;
            Host = host;
            Path = path;
        }

        public string Name { get; private set; }
        public string Host { get; private set; }
        public string Path { get; private set; }
    }
}