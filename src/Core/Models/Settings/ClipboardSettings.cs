using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace Seemon.Vault.Core.Models.Settings
{
    public class ClipboardSettings : ObservableObject
    {
        public enum CopyActions
        {
            [Description("No clipboard")]
            None,
            [Description("Always copy to clipboard")]
            Always,
            [Description("On-demand copy to clipboard")]
            OnDemand
        }

        private CopyActions _copy;
        private bool _autoClear;
        private int _autoClearDuration;

        [JsonProperty("copy")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CopyActions Copy
        {
            get => _copy; set => SetProperty(ref _copy, value);
        }

        [JsonProperty("autoClear")]
        public bool AutoClear
        {
            get => _autoClear; set => SetProperty(ref _autoClear, value);
        }

        [JsonProperty("autoClearDuration")]
        public int AutoClearDuration
        {
            get => _autoClearDuration; set => SetProperty(ref _autoClearDuration, value);
        }

        public static ClipboardSettings Default => new() 
        { 
            Copy = CopyActions.OnDemand, 
            AutoClear = false, 
            AutoClearDuration = 10 
        };
    }
}
