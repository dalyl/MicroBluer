
namespace MicroBluer.AndroidUtils.IO
{
    using Java.IO;
    using Java.Lang;

    public class SystemManager
    {
        /// <summary>
        ///  应用程序运行命令获取 Root权限，设备必须已破解(获得ROOT权限)
        ///  @param command 命令：String apkRoot = "chmod 777 " + getPackageCodePath(); RootCommand(apkRoot);
        ///  @return 应用程序是/否获取Root权限
        /// </summary>
        public static bool RootCommand(string command)
        {
            Process process = null;
            try
            {
                process = Runtime.GetRuntime().Exec("su");
                using (var os = new DataOutputStream(process.OutputStream))
                {
                    os.WriteBytes(command + "\n");
                    os.WriteBytes("exit\n");
                    os.Flush();
                }
                process.WaitFor();
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                if (process != null) process.Destroy();
            }
            return true;
        }
    }
}