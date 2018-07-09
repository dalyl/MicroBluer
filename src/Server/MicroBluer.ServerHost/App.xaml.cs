namespace MicroBluer.ServerHost
{
    using System.Windows;
    using MicroBluer.ServerCore.NamedPipe;
    using MicroBluer.ServerHost.Service;
    using MicroBluer.ServerHost.Core;
    using MicroBluer.Windows.Notification.Model;
    using MicroBluer.Windows.Notification.Services;
    using MicroBluer.Common;

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        PipeServer PipeServer { get; }

        public App() {
            TryNotice.InitCurrent(ShowMessage);
            PipeServer = new PipeServer(new WindowsCommandDistributor());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProcess.Instance.Stop(null);
        }

        void Notificatiom(string title, string messsge)
        {
            INotificationDialogService _dailogService = new NotificationDialogService();
            var newNotification = new Notification()
            {
                Title = title,
                Message = messsge,
            };
            _dailogService.ShowNotificationWindow(newNotification);
        }

        void ShowMessage( string messsge)
        {
            INotificationDialogService _dailogService = new NotificationDialogService();
            var newNotification = new Notification()
            {
                Title = "",
                Message = messsge,
            };
            _dailogService.ShowNotificationWindow(newNotification);
        }


    }
}
    
