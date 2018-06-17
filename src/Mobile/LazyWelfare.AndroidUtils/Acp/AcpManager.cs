namespace LazyWelfare.AndroidUtils.Acp
{
    using System;
    using System.Collections.Generic;
    using Activity = Android.App.Activity;
    using AlertDialog = Android.App.AlertDialog;
	using ActivityNotFoundException = Android.Content.ActivityNotFoundException;
	using Context = Android.Content.Context;
	using Intent = Android.Content.Intent;
	using PackageInfo = Android.Content.PM.PackageInfo;
	using PackageManager = Android.Content.PM.PackageManager;
	using Uri = Android.Net.Uri;
	using Build = Android.OS.Build;
	using Settings = Android.Provider.Settings;
    using System.Linq;
    using Android.Content.PM;
    using Android.App;
    using Android.Content;
    using Android.OS;


    /// <summary>
    /// Created by hupei on 2016/4/26.
    /// </summary>
    internal class AcpManager
	{
		private const string TAG = "AcpManager";
		private const int REQUEST_CODE_PERMISSION = 0x38;
		private const int REQUEST_CODE_SETTING = 0x39;
		private Context mContext;
		private Activity mActivity;
		private AcpService mService;
		private AcpOptions mOptions;
		private IAcpListener mCallback;
		private readonly LinkedList<string> mDeniedPermissions = new LinkedList<string>();
		private readonly HashSet<string> mManifestPermissions = new HashSet<string>(1);

		internal AcpManager(Context context)
		{
			mContext = context;
			mService = new AcpService();
            GetManifestPermissions();
		}

        private void GetManifestPermissions()
        {
            lock (this)
            {
                PackageInfo packageInfo = null;
                try
                {
                    packageInfo = mContext.PackageManager.GetPackageInfo(mContext.PackageName, PackageInfoFlags.Permissions);
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                if (packageInfo != null && packageInfo.RequestedPermissions != null)
                {
                    foreach (string perm in packageInfo.RequestedPermissions)
                    {
                        mManifestPermissions.Add(perm);
                    }
                }
            }
        }

		/// <summary>
		/// 开始请求
		/// </summary>
		/// <param name="options"> </param>
		/// <param name="acpListener"> </param>
		internal virtual void Request(AcpOptions options, IAcpListener acpListener)
		{
			lock (this)
			{
				mCallback = acpListener;
				mOptions = options;
				CheckSelfPermission();
			}
		}

		/// <summary>
		/// 检查权限
		/// </summary>
		private void CheckSelfPermission()
		{
			lock (this)
			{
				mDeniedPermissions.Clear();
				if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                {
					if (mCallback != null)
					{
						mCallback.OnGranted();
					}
					OnDestroy();
					return;
				}
				string[] permissions = mOptions.Permissions;
				foreach (string permission in permissions)
				{
					//检查申请的权限是否在 AndroidManifest.xml 中
					if (mManifestPermissions.Contains(permission))
					{
						var checkSelfPermission = mService.CheckSelfPermission(mContext, permission);
						//如果是拒绝状态则装入拒绝集合中
						if (checkSelfPermission == Permission.Denied)//PackageManager.PERMISSION_DENIED
                        {
							mDeniedPermissions.AddLast(permission);
						}
					}
				}
				//检查如果没有一个拒绝响应 onGranted 回调
				if (mDeniedPermissions.Count == 0)
				{
					if (mCallback != null)
					{
						mCallback.OnGranted();
					}
					OnDestroy();
					return;
				}
				StartAcpActivity();
			}
		}

		/// <summary>
		/// 启动处理权限过程的 Activity
		/// </summary>
		private void StartAcpActivity()
		{
			lock (this)
			{
				Intent intent = new Intent(mContext, typeof(AcpActivity));
				intent.AddFlags( ActivityFlags.NewTask);//Intent.InterfaceConsts.FLAG_ACTIVITY_NEW_TASK
                mContext.StartActivity(intent);
			}
		}

		/// <summary>
		/// 检查权限是否存在拒绝不再提示
		/// </summary>
		internal virtual void CheckRequestPermissionRationale(Activity activity)
		{
			lock (this)
			{
				mActivity = activity;
				bool rationale = false;
				//如果有拒绝则提示申请理由提示框，否则直接向系统请求权限
				foreach (string permission in mDeniedPermissions)
				{
					rationale = rationale || mService.ShouldShowRequestPermissionRationale(mActivity, permission);
				}
				string[] permissions = mDeniedPermissions.ToArray();
				if (rationale)
				{
					ShowRationalDialog(permissions);
				}
				else
				{
					RequestPermissions(permissions);
				}
			}
		}

		/// <summary>
		/// 申请理由对话框
		/// </summary>
		private void ShowRationalDialog(string[] permissions)
		{
			lock (this)
			{
				AlertDialog alertDialog = (new AlertDialog.Builder(mActivity))
                    .SetMessage(mOptions.RationalMessage)
                    .SetPositiveButton(mOptions.RationalBtnText, (s,e)=> RequestPermissions(permissions))
					.Create();
				alertDialog.SetCancelable (mOptions.DialogCancelable);
				alertDialog.SetCanceledOnTouchOutside(mOptions.DialogCanceledOnTouchOutside);
				alertDialog.Show();
			}
		}

		/// <summary>
		/// 向系统请求权限
		/// </summary>
		private void RequestPermissions(string[] permissions)
		{
			lock (this)
			{
				mService.RequestPermissions(mActivity, permissions, REQUEST_CODE_PERMISSION);
			}
		}

		/// <summary>
		/// 响应向系统请求权限结果
		/// </summary>
		internal virtual void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			lock (this)
			{
				switch (requestCode)
				{
					case REQUEST_CODE_PERMISSION:
						LinkedList<string> grantedPermissions = new LinkedList<string>();
						LinkedList<string> deniedPermissions = new LinkedList<string>();
						for (int i = 0; i < permissions.Length; i++)
						{
							string permission = permissions[i];
							if (grantResults[i] ==   Permission.Granted) 
                            {
								grantedPermissions.AddLast(permission);
							}
							else
							{
								deniedPermissions.AddLast(permission);
							}
						}
						//全部允许才回调 onGranted 否则只要有一个拒绝都回调 onDenied
						if (grantedPermissions.Count > 0 && deniedPermissions.Count == 0)
						{
							if (mCallback != null)
							{
								mCallback.OnGranted();
							}
							OnDestroy();
						}
						else if (deniedPermissions.Count > 0)
						{
                            ShowDeniedDialog(deniedPermissions);
						}
						break;
				}
			}
		}

        /// <summary>
        /// 拒绝权限提示框
        /// </summary>
        private void ShowDeniedDialog(LinkedList<string> permissions)
        {
            lock (this)
            {
                AlertDialog alertDialog = (new AlertDialog.Builder(mActivity)).SetMessage(mOptions.DeniedMessage)
                       .SetNegativeButton(mOptions.DeniedCloseBtn, (s, e) => {
                           if (this.mCallback != null)
                           {
                               this.mCallback.OnDenied(permissions);
                           }
                           this.OnDestroy();
                       }).SetPositiveButton(mOptions.DeniedSettingBtn, (s, e) => this.StartSetting()).Create();
                alertDialog.SetCancelable(mOptions.DialogCancelable);
                alertDialog.SetCanceledOnTouchOutside(mOptions.DialogCanceledOnTouchOutside);
                alertDialog.Show();
            }
        }
	

		/// <summary>
		/// 摧毁本库的 AcpActivity
		/// </summary>
		private void OnDestroy()
		{
			if (mActivity != null)
			{
				mActivity.Finish();
				mActivity = null;
			}
			mCallback = null;
		}

		/// <summary>
		/// 跳转到设置界面
		/// </summary>
		private void StartSetting()
		{
			if (MiuiOs.MIUI)
			{
				Intent intent = MiuiOs.GetSettingIntent(mActivity);
				if (MiuiOs.IsIntentAvailable(mActivity, intent))
				{
					mActivity.StartActivityForResult(intent, REQUEST_CODE_SETTING);
					return;
				}
			}
			try
			{
				Intent intent = (new Intent(Settings.ActionApplicationDetailsSettings)).SetData(Uri.Parse("package:" + mActivity.PackageName));
				mActivity.StartActivityForResult(intent, REQUEST_CODE_SETTING);
			}
			catch (ActivityNotFoundException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				try
				{
					Intent intent = new Intent(Settings.ActionManageApplicationsSettings);
					mActivity.StartActivityForResult(intent, REQUEST_CODE_SETTING);
				}
				catch (Exception e1)
				{
					Console.WriteLine(e1.ToString());
					Console.Write(e1.StackTrace);
				}
			}

		}

		/// <summary>
		/// 响应设置权限返回结果
		/// </summary>
		/// <param name="requestCode"> </param>
		/// <param name="resultCode"> </param>
		/// <param name="data"> </param>
		internal virtual void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			lock (this)
			{
				if (mCallback == null || mOptions == null || requestCode != REQUEST_CODE_SETTING)
				{
					OnDestroy();
					return;
				}
				CheckSelfPermission();
			}
		}
	}

}