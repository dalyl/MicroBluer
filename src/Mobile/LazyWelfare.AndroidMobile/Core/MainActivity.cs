namespace LazyWelfare.AndroidMobile
{
    using LazyWelfare.AndroidMobile.Views;
    using LazyWelfare.AndroidMobile.Views.Partials;
    using LazyWelfare.AndroidMobile.Script;
    using LazyWelfare.AndroidMobile.Logic;
    using Android.App;
    using Android.OS;
    using Android.Webkit;

    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : PartialActivity
    {
        static PartialRequestStack requestStack { get; set; } = new PartialRequestStack();

        public (string Host, string Path) Partial { get; set; } = HomeView.Partial;

        public override PartialRequestStack RequestStack { get; } = requestStack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            PartialView = FindViewById<WebView>(Resource.Id.webView);

            WebSettings settings = PartialView.Settings;
            
            //启用js事件  
            settings.SetSupportZoom(true);
            settings.JavaScriptEnabled = true;
            //启用js的dom缓存  
            settings.DomStorageEnabled = true;
            //加载javascript接口方法，以便调用前台方法  
            PartialView.AddJavascriptInterface(new AndroidScript(this, PartialView), "AndroidScript");
            PartialView.AddJavascriptInterface(new PartialScript(this, PartialView), "PartialScript");
            PartialView.AddJavascriptInterface(new BuinessScript(this, PartialView), "BuinessScript");

            PartialView.SetWebViewClient(new AgreementRouteClient($"ViewScript.RequestPartial('#MainContent','{PartialLoadForm.Replace}' ,'{Partial.Host}','{Partial.Path}',null);"));
            
            var model = ActiveContext.Current.UserStore.Get();
            var host = string.IsNullOrEmpty(model.Host) ? null : ActiveContext.Current.HostStore.Get(model.Host);
            ActiveContext.CurrentHost = host;
            var page = Template.Layout(model.Name);
            PartialView.LoadDataWithBaseURL("file:///android_asset/", page, "text/html", "UTF-8", null);
          
        }
       
    }



}