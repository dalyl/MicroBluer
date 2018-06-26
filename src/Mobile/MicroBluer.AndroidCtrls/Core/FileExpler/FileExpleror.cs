using System;
namespace MicroBluer.AndroidCtrls.FileExpler
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Android.Content;

    public class FileExpleror
    {
        public static void OpenDialog(Context context, string[] extensions = null)
        {
            FileExplerorActivity.OnCreateStart = the => {
                the.Extensions = new string[] { ".jpg"};
            };
            var selectorIntent = new Intent(context, typeof(FileExplerorActivity));
            context.StartActivity(selectorIntent);
        }

        public static void OpenDialog(Context context,List<string> dirs,string[] extensions=null)
        {
            FileExplerorActivity.OnCreateStart = the => {
                if (dirs != null && dirs.Count > 0) the.Roots = dirs;
                the.Extensions = extensions;
            };
            var selectorIntent = new Intent(context, typeof(FileExplerorActivity));
            context.StartActivity(selectorIntent);
        }
    }
}