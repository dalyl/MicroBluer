using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MicroBluer.AndroidMobile.Core
{
    public class PermissionGrantedClickListener : Java.Lang.Object, View.IOnClickListener
    {
        Action clickEvent { get; set; }

        Activity context { get; set; }

        public PermissionGrantedClickListener(Activity activity)
        {
            context = activity;
        }

        public PermissionGrantedClickListener(Activity activity,Action invoke)
        {
            clickEvent = invoke;
            context = activity;
        }
        public void OnClick(View v)
        {
            if (!IsGrantExternalRW(context))
            {
                return;
            }
            clickEvent?.Invoke();
        }

        public bool IsGrantExternalRW(Activity activity)
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M && activity.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
            {
                activity.RequestPermissions(new String[] { Android.Manifest.Permission.ReadExternalStorage, Android.Manifest.Permission.WriteExternalStorage }, 1);
                return false;
            }

            return true;
        }
    }

}