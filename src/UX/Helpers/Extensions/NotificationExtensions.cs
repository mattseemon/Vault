using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Controls.Notifications;
using Seemon.Vault.Models;

namespace Seemon.Vault.Helpers.Extensions
{
    public static class NotificationExtensions
    {
        public static NotificationMessageBuilder WithBadgeType(this NotificationMessageBuilder builder, NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    builder.SetBadge("INFO");
                    builder.SetAccent(Constants.NOTIFICATION_COLOR_INFO);
                    break;
                case NotificationType.Warning:
                    builder.SetBadge("WARN");
                    builder.SetAccent(Constants.NOTIFICATION_COLOR_WARN);
                    break;
                case NotificationType.Error:
                    builder.SetBadge("ERROR");
                    builder.SetAccent(Constants.NOTIFICATION_COLOR_ERROR);
                    break;
                case NotificationType.Update:
                    builder.SetBadge("UPDATE");
                    builder.SetAccent(Constants.NOTIFICATION_COLOR_UPDATE);
                    break;
                case NotificationType.None:
                    break;
            }
            return builder;
        }

        public static void Dismiss(this INotificationMessage message)
        {
            var notificationService = App.GetService<INotificationService>();
            notificationService?.Manager.Dismiss(message);
        }
    }
}
