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

namespace LazyWelfare.AndroidMobile.AgreementServices
{
    public abstract class AgreementService
    {
        static Dictionary<string, Type> Servces = new Dictionary<string, Type>();

        static AgreementService()
        {
            Servces.Add(nameof(IVolumeController), typeof(VolumeService));
        }

        public static bool Contains(string service) => Servces.Keys.Contains(service);

        public static bool Execute(Context context)
        {
            var ctrl = new VolumeService(context);
            ctrl.Show();
            return true;
        }
    }
}