using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Controls.Notifications;
using Seemon.Vault.Helpers.Extensions;
using Seemon.Vault.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Seemon.Vault.Services
{
    public class NotificationService : INotificationService
    {
        private INotificationMessageManager _manager;

        public INotificationMessageManager Manager => _manager;

        public NotificationService() => _manager = new NotificationMessageManager();

        public NotificationMessageBuilder Default()
        {
            var builder = _manager.CreateMessage()
                .Background("#333")
                .Animates(true);

            return builder;
        }

        public INotificationMessage ShowMessage(NotificationType type, string message, string header = null, Action closeAction = null)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return Default()
                    .HasHeader(header)
                    .HasMessage(message)
                    .Dismiss().WithButton("Close", button => { closeAction?.Invoke(); })
                    .WithBadgeType(type)
                    .Queue();
            }, DispatcherPriority.Normal);
        }

        public Task<string> ActionSheet(string message, IDictionary<string, string> actions)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<string>();

                var builder = Default()
                    .HasMessage(message);

                foreach (var action in actions)
                {
                    builder = action.Key.StartsWith(@"-")
                        ? builder.WithButton(action.Value, button => { taskCompletionSource.TrySetResult(action.Key); })
                        : builder.Dismiss().WithButton(action.Value, button => { taskCompletionSource.TrySetResult(action.Key); });
                }

                builder.Queue();

                return taskCompletionSource.Task;
            }, DispatcherPriority.Normal);
        }
    }
}
