
namespace LazyWelfare.AndroidUtils.Acp
{
    using System;
    using System.IO;
	using Context = Android.Content.Context;
	using Intent = Android.Content.Intent;
	using Build = Android.OS.Build;
	using TextUtils = Android.Text.TextUtils;
    using System.Diagnostics;

    public class MiuiOs
	{
		public const string UNKNOWN = "UNKNOWN";

		/// <summary>
		/// 获取应用权限设置 Intent 
		/// </summary>
		public static Intent GetSettingIntent(Context context)
		{
			// 之兼容miui v5/v6/v7  的应用权限设置页面
			if (SystemVersionCode >= 6)
			{
				Intent intent = new Intent("miui.intent.action.APP_PERM_EDITOR");
				intent.SetClassName("com.miui.securitycenter", "com.miui.permcenter.permissions.AppPermissionsEditorActivity");
				intent.PutExtra("extra_pkgname", context.PackageName);
				return intent;
			}
			return null;
		}

		/// <summary>
		/// 获取 V5/V6 后面的数字
		/// </summary>
		public static int SystemVersionCode
		{
			get
			{
				string systemProperty = SystemProperty;
				if (!TextUtils.IsEmpty(systemProperty) && !systemProperty.Equals(UNKNOWN) && systemProperty.Length == 2 && systemProperty.ToUpper().StartsWith("V", StringComparison.Ordinal))
				{
					int? code = 0;
					try
					{
						code = Convert.ToInt32(systemProperty.Substring(1));
					}
					catch (System.FormatException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
					return code.Value;
				}
				return 0;
			}
		}

		/// <summary>
		/// 判断V5/V6
		/// </summary>
		/// <returns> V5 、V6 、V7 字符 </returns>
		public static string SystemProperty
		{
			get
			{
				string line = UNKNOWN;
				StreamReader reader = null;
				try
				{
					Process p = Process.Start("getprop ro.miui.ui.version.name");
					reader = p.StandardOutput;
					line = reader.ReadLine();
					return line;
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				finally
				{
					if (reader != null)
					{
						try
						{
							reader.Close();
						}
						catch (IOException e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
					}
				}
				return line;
			}
		}

		/// <summary>
		/// 判断 activity 是否存在
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="intent">
		/// @return </param>
		public static bool IsIntentAvailable(Context context, Intent intent)
		{
			if (intent == null)
			{
				return false;
			}
			return context.PackageManager.QueryIntentActivities(intent, 0).Count > 0;
		}

		/// <summary>
		/// 检查手机是否是miui
		/// </summary>
		public static bool MIUI
		{
			get
			{
				string device = Build.Manufacturer;
				if (device.Equals("Xiaomi"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}

}