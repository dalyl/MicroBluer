namespace MicroBluer.AndroidUtils.Acp
{
    using Android.Views;
    using Android.Content.PM;
    using Intent = Android.Content.Intent;
	using Bundle = Android.OS.Bundle;
    using Android.App;
    [Activity(Theme = "@android:style/Theme.NoTitleBar")]
    public class AcpActivity : Activity
	{

		protected  override void OnCreate(Bundle savedInstanceState)
		{

            base.OnCreate(savedInstanceState);
			//不接受触摸屏事件
			Window.AddFlags( WindowManagerFlags.NotTouchable );

            if (savedInstanceState == null)
			{
				Acp.getInstance(this).AcpManager.CheckRequestPermissionRationale(this);
			}
		}

		protected  override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
			Acp.getInstance(this).AcpManager.CheckRequestPermissionRationale(this);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			Acp.getInstance(this).AcpManager.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

        protected override void OnActivityResult(int requestCode,  Result resultCode, Intent data)
        {
            Acp.getInstance(this).AcpManager.OnActivityResult(requestCode, resultCode, data);
        }
	}

}