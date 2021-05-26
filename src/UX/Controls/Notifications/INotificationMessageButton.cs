/************************************************************************************
* Author: https://github.com/Enterwell                                              *
* Availability: https://github.com/Enterwell/Wpf.Notifications                      *
* License: MIT (https://github.com/Enterwell/Wpf.Notifications/blob/master/LICENSE) *
************************************************************************************/
using System;

namespace Seemon.Vault.Controls.Notifications
{
    /// <summary>
    /// The notification message button.
    /// </summary>
    public interface INotificationMessageButton
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        object Content { get; set; }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        Action<INotificationMessageButton> Callback { get; set; }
    }
}