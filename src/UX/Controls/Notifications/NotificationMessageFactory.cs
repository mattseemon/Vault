/************************************************************************************
* Author: https://github.com/Enterwell                                              *
* Availability: https://github.com/Enterwell/Wpf.Notifications                      *
* License: MIT (https://github.com/Enterwell/Wpf.Notifications/blob/master/LICENSE) *
************************************************************************************/
namespace Seemon.Vault.Controls.Notifications
{
    /// <summary>
    /// The notification message factory.
    /// </summary>
    /// <seealso cref="INotificationMessageFactory" />
    public class NotificationMessageFactory : INotificationMessageFactory
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// Returns new instance of notification message.
        /// </returns>
        public INotificationMessage GetMessage() => new NotificationMessage();

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <returns>
        /// Returns new instance of notification message button.
        /// </returns>
        public INotificationMessageButton GetButton() => new NotificationMessageButton();
    }
}