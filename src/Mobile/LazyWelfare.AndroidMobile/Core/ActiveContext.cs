namespace LazyWelfare.AndroidMobile
{
    using Android.App;
    using Android.Widget;

    public class ActiveContext
    {
        public static ActiveContext Current { get; set; }

        public static TryCatch Try { get; } = new TryCatch(ShowMessage);

        static void ShowMessage(string message)
        {
            var Activity = Current.ActivityContext;
            Activity.RunOnUiThread(() => Toast.MakeText(Activity, message.Trim(), ToastLength.Short).Show());
        }

        public Activity ActivityContext { get; set; }

    }

}