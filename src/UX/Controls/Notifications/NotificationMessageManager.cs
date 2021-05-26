/************************************************************************************
* Author: https://github.com/Enterwell                                              *
* Availability: https://github.com/Enterwell/Wpf.Notifications                      *
* License: MIT (https://github.com/Enterwell/Wpf.Notifications/blob/master/LICENSE) *
************************************************************************************/
using System.Collections.Generic;

namespace Seemon.Vault.Controls.Notifications
{
    /// <summary>
    /// The notification message manager.
    /// </summary>
    /// <seealso cref="INotificationMessageManager" />
    public class NotificationMessageManager : INotificationMessageManager
    {
        private readonly List<INotificationMessage> queuedMessages = new List<INotificationMessage>();


        /// <summary>
        /// Occurs when new notification message is queued.
        /// </summary>
        public event NotificationMessageManagerEventHandler OnMessageQueued;

        /// <summary>
        /// Occurs when notification message is dismissed.
        /// </summary>
        public event NotificationMessageManagerEventHandler OnMessageDismissed;

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>
        /// The factory.
        /// </value>
        public INotificationMessageFactory Factory { get; set; } = new NotificationMessageFactory();


        /// <summary>
        /// Queues the specified message.
        /// This will ignore the <c>null</c> message or already queued notification message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Queue(INotificationMessage message)
        {
            if (message == null || this.queuedMessages.Contains(message))
                return;

            this.queuedMessages.Add(message);

            TriggerMessageQueued(message);
        }

        /// <summary>
        /// Triggers the message queued event.
        /// </summary>
        /// <param name="message">The message.</param>
        private void TriggerMessageQueued(INotificationMessage message) => OnMessageQueued?.Invoke(this, new NotificationMessageManagerEventArgs(message));

        /// <summary>
        /// Dismisses the specified message.
        /// This will ignore the <c>null</c> or not queued notification message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Dismiss(INotificationMessage message)
        {
            if (message == null || !this.queuedMessages.Contains(message))
                return;

            this.queuedMessages.Remove(message);

            if (message is INotificationAnimation animatableMessage)
            {
                var animation = animatableMessage.AnimationOut;
                if (animation != null &&
                    animatableMessage.Animates &&
                    animatableMessage.AnimatableElement != null &&
                    animatableMessage.AnimationOutDependencyProperty != null)
                {
                    animation.Completed += (s, a) =>
                    {
                        TriggerMessageDismissed(message);
                    };
                    animatableMessage.AnimatableElement.BeginAnimation(animatableMessage.AnimationOutDependencyProperty, animation);
                }
                else
                    TriggerMessageDismissed(message);
            }
            else
                TriggerMessageDismissed(message);
        }

        /// <summary>
        /// Triggers the message dismissed event.
        /// </summary>
        /// <param name="message">The message.</param>
        private void TriggerMessageDismissed(INotificationMessage message) => OnMessageDismissed?.Invoke(this, new NotificationMessageManagerEventArgs(message));
    }
}
