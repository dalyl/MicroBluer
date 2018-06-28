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

namespace MicroBluer.AndroidMobile.WebAgreement
{
    public struct AgreementUri
    {
        public static AgreementUri Empty = new AgreementUri();
        public AgreementUri(string name, string host, string path, string args = "")
        {
            Name = name;
            Host = host;
            Path = path;
            Args = args;
        }

        public AgreementUri(AgreementUri uri, string args)
        {
            Name = uri.Name;
            Host = uri.Host;
            Path = uri.Path;
            Args = args;
        }

        public string Name { get; private set; }
        public string Host { get; private set; }
        public string Path { get; private set; }
        public string Args { get; private set; }

        public static bool operator ==(AgreementUri c1, AgreementUri c2)
        {
            return c1.Name == c2.Name;
        }

        public static bool operator !=(AgreementUri c1, AgreementUri c2)
        {
            return c1.Name != c2.Name;
        }
    }
}