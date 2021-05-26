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
    public interface INotificationMessageFactory
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>Returns new instance of notification message.</returns>
        INotificationMessage GetMessage();

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <returns>Returns new instance of notification message button.</returns>
        INotificationMessageButton GetButton();
    }
}