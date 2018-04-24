using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    

    public class CommandTask
    {
        public bool IsRunning { get;  private set; } = false;

        public Func<string, bool, bool> ShowMessage { get; set; }

        public System.Diagnostics.Process Process { get; private set; } = new System.Diagnostics.Process();

        public string Path{get; private set; }

        public CommandTask() { }

        public CommandTask(string path)
        {
            Path = path;
        }

        public Task Exec(string cmd)
        {
            if (IsRunning) return Task.FromException(new Exception("服务正在运行"));
            RefreshCmdState(this, true);
            return Task.Run(() =>
            {
                Process.StartInfo.FileName = "cmd.exe";
                Process.StartInfo.WorkingDirectory = Path;
                Process.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                Process.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            //    Process.StartInfo.RedirectStandardOutput = false; //由调用程序获取输出信息
            //    Process.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                Process.StartInfo.CreateNoWindow = false;         //不显示程序窗口
                Process.Start();                                  //启动程序
                                                                  //  Process.StandardInput.WriteLine(cmd + "&exit");   //向cmd窗口发送输入信息
                Process.StandardInput.WriteLine(cmd);   //向cmd窗口发送输入信息
                                                        //  Process.StandardInput.WriteLine("");              //向cmd窗口发送输入信息
                                                        //  Process.WaitForExit();                            //等待程序执行完退出进程
                                                        //  Process.Close();
                                                        //  RefreshCmdState(this, false);
            });
        }

       
        void RefreshCmdState(CommandTask task, bool state)
        {
            task.IsRunning = state;
      //      ShowMessage($"\t 命令行：{(state ? "被占用" : "空闲")}",true);
        }

      
    }
}
