namespace LazyWelfare.AndroidMobile
{
    using Android.App;
    using Android.OS;

    public abstract class ActiveActivity : Activity
    {
        ActiveContext ActiveInfo { get; set; }

        public ActiveActivity()
        {
            ActiveInfo = new ActiveContext(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ActiveContext.RegisterContext(ActiveInfo);
        }

        protected override void OnResume()
        {
            base.OnResume();
            ActiveContext.RepalceContext(ActiveInfo);
        }

        protected override void OnDestroy()
        {
            ActiveContext.ExprieContext();
            ActiveInfo = null;
            base.OnDestroy();
        }

    }
}