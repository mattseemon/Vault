using Seemon.Vault.Controls.Notifications;
using Seemon.Vault.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seemon.Vault.Contracts.Services
{
    public interface INotificationService
    {
        INotificationMessageManager Manager { get; }

        INotificationMessage ShowMessage(NotificationType type, string message, string header = null, Action closeAction = null);

        Task<string> ActionSheet(string message, IDictionary<string, string> actions);

        NotificationMessageBuilder Default();
    }
}
