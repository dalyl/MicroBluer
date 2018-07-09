using MicroBluer.Windows.Notification.Core.Configuration;

namespace MicroBluer.Windows.Notification.Services
{
    public interface INotificationDialogService
    {
        /// <summary>
        /// Show notification window.
        /// </summary>
        /// <param name="content">The notification object.</param>
        void ShowNotificationWindow(object content);

        /// <summary>
        /// Show notification window.
        /// </summary>
        /// <param name="content">The notification object.</param>
        /// <param name="configuration">The notification configuration object.</param>
        void ShowNotificationWindow(object content, NotificationConfiguration configuration);

        /// <summary>
        /// Remove all notifications from notification list and buffer list.
        /// </summary>
        void ClearNotifications();
    }
}