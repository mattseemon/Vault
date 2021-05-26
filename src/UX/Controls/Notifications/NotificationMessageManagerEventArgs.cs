/************************************************************************************
* Author: https://github.com/Enterwell                                              *
* Availability: https://github.com/Enterwell/Wpf.Notifications                      *
* License: MIT (https://github.com/Enterwell/Wpf.Notifications/blob/master/LICENSE) *
************************************************************************************/
using System;

namespace Seemon.Vault.Controls.Notifications
{
    /// <summary>
    /// The notification message manager event arguments.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class NotificationMessageManagerEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public INotificationMessage Message { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessageManagerEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NotificationMessageManagerEventArgs(INotificationMessage message) => this.Message = message;
    }
}
