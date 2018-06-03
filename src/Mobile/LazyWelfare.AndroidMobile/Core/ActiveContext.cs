namespace LazyWelfare.AndroidMobile
{
    using Android.App;
    using Android.Widget;
    using LazyWelfare.AndroidMobile.Logic;
    using LazyWelfare.AndroidMobile.Models;

    public class ActiveContext
    {
        static ActiveContext _current = null;

        static HostModel _host { get; set; }

        public static ActiveContext Current
        {
            get { return _current; }
            set { InitService(value); }
        }

        public static HostModel CurrentHost
        {
            get { return _host; }
            set {
                _host = value;
                _current.Host = new HostExpressService(CurrentHost);
            }
        }

        public ActiveContext(Activity context)
        {
            ActivityContext = context;
        }

        public static TryCatch Try { get; } = new TryCatch(ShowMessage);

        static void ShowMessage(string message)
        {
            var Activity = Current.ActivityContext;
            Activity.RunOnUiThread(() => Toast.MakeText(Activity, message.Trim(), ToastLength.Short).Show());
        }

        static void InitService(ActiveContext value)
        {
            _current = value;
            _current.HostStore = new HostStoreService();
            _current.UserStore = new UserStoreService();
        }

        public Activity ActivityContext { get; private set; }

        public HostStoreService HostStore { get; private set; }

        public UserStoreService UserStore { get; private set; }

        internal HostExpressService Host { get; private set; }

    }

}