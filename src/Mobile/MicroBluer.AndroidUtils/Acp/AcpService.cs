namespace MicroBluer.AndroidUtils.Acp
{
    using System;
    using Android.Content.PM;
    using Activity = Android.App.Activity;
    using Context = Android.Content.Context;
	using PackageInfo = Android.Content.PM.PackageInfo;
	using PackageManager = Android.Content.PM.PackageManager;
	using Build = Android.OS.Build;
	using ActivityCompat = Android.Support.V4.App.ActivityCompat;
	using ContextCompat = Android.Support.V4.Content.ContextCompat;
	using PermissionChecker = Android.Support.V4.Content.PermissionChecker;

	/// <summary>
	/// Created by hupei on 2016/4/26.
	/// </summary>
	internal class AcpService
	{
		private const string TAG = "AcpService";

        /// <summary>
        /// 检查权限授权状态
        /// </summary>
        internal virtual Permission CheckSelfPermission(Context context, string permission)
        {
            try
            {
                PackageInfo info = context.PackageManager.GetPackageInfo(context.PackageName, 0);
                var targetSdkVersion = info.ApplicationInfo.TargetSdkVersion;
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                {
                    if (targetSdkVersion >= Android.OS.BuildVersionCodes.M)
                    {
                        return ContextCompat.CheckSelfPermission(context, permission);
                    }
                    else
                    {
                        return (Permission)PermissionChecker.CheckSelfPermission(context, permission);
                    }
                }
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            return ContextCompat.CheckSelfPermission(context, permission);
        }

		/// <summary>
		/// 向系统请求权限
		/// </summary>
		internal virtual void RequestPermissions(Activity activity, string[] permissions, int requestCode)
		{
			ActivityCompat.RequestPermissions(activity, permissions, requestCode);
		}

		/// <summary>
		/// 检查权限是否存在拒绝不再提示
		/// </summary>
		internal virtual bool ShouldShowRequestPermissionRationale(Activity activity, string permission)
		{
			bool shouldShowRational = ActivityCompat.ShouldShowRequestPermissionRationale(activity, permission);
			return shouldShowRational;
		}
	}

}