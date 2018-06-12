using System;
namespace LazyWelfare.AndroidCtrls.FileExpler
{
    using System.Threading;
    using System.Threading.Tasks;
    using Android.Content;

    public class FileExpleror
    {
        public static void OpenDialog(Context context)
        {
            var selectorIntent = new Intent(context, typeof(FileExplerorActivity));
            context.StartActivity(selectorIntent);
        }
    }
}