namespace LazyWelfare.AndroidMobile
{
    using Android.App;
    using Android.OS;

    public abstract class ActiveActivity : Activity
    {
        ActiveContext ActiveInfo { get; set; }

        public ActiveActivity()
        {
            ActiveInfo = new ActiveContext
            {
                ActivityContext = this,
            };
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(ActiveContext.Current==null) ActiveContext.Current = ActiveInfo;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ActiveContext.Current = ActiveInfo;
        }

        protected override void OnDestroy()
        {
            if (ActiveContext.Current == ActiveInfo) ActiveContext.Current = null;
            ActiveInfo = null;
            base.OnDestroy();
        }

    }
}