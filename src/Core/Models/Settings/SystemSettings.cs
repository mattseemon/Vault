using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Seemon.Vault.Core.Models.Settings
{
    public class SystemSettings : ObservableObject
    {
        private bool _startWithWindows = false;
        private bool _alwaysOnTop = false;
        private bool _showVaultInNotificationArea = false;
        private bool _minimizeToNotificationArea = false;
        private bool _closeToNotificationArea = false;

        [JsonProperty("startWithWindows")]
        public bool StartWithWindows
        {
            get => _startWithWindows; set => SetProperty(ref _startWithWindows, value);
        }

        [JsonProperty("alwaysOnTop")]
        public bool AlwaysOnTop
        {
            get => _alwaysOnTop; set => SetProperty(ref _alwaysOnTop, value);
        }

        [JsonProperty("showVaultInNotificationArea")]
        public bool ShowVaultInNotificationArea
        {
            get => _showVaultInNotificationArea; set => SetProperty(ref _showVaultInNotificationArea, value);
        }

        [JsonProperty("minimizeToNotificationArea")]
        public bool MinimizeToNotificationArea
        {
            get => _minimizeToNotificationArea; set => SetProperty(ref _minimizeToNotificationArea, value);
        }

        [JsonProperty("closeToNotificationArea")]
        public bool CloseToNotificationArea
        {
            get => _closeToNotificationArea; set => SetProperty(ref _closeToNotificationArea, value);
        }

        public static SystemSettings Default => new() 
        { 
            StartWithWindows = false, 
            AlwaysOnTop = false, 
            ShowVaultInNotificationArea = false, 
            MinimizeToNotificationArea = false, 
            CloseToNotificationArea = false 
        };
    }
}
