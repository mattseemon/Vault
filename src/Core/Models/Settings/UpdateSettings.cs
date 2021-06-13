using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace Seemon.Vault.Core.Models.Settings
{
    public class UpdateSettings : ObservableObject
    {
        public enum CheckUpdateOptions
        {
            [Description("Every time I start the application")]
            ApplicationStart,
            [Description("Once a week")]
            Weekly,
            [Description("Once every two weeks")]
            BiWeekly,
            [Description("Once a month")]
            Monthly
        }

        private bool _autoUpdate;
        private CheckUpdateOptions _checkUpdates;
        private bool _includePreReleases;
        private bool _showReleaseNotes;
        private DateTime? _lastChecked;
        private DateTime? _lastUpdated;

        [JsonProperty("autoUpdate")]
        public bool AutoUpdate
        {
            get => _autoUpdate; set => SetProperty(ref _autoUpdate, value);
        }

        [JsonProperty("checkUpdates")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CheckUpdateOptions CheckUpdates
        {
            get => _checkUpdates; set => SetProperty(ref _checkUpdates, value);
        }

        [JsonProperty("includePreReleases")]
        public bool IncludePreReleases
        {
            get => _includePreReleases; set => SetProperty(ref _includePreReleases, value);
        }

        [JsonProperty("showReleaseNotes")]
        public bool ShowReleaseNotes
        {
            get => _showReleaseNotes; set => SetProperty(ref _showReleaseNotes, value);
        }

        [JsonProperty("lastChecked")]
        public DateTime? LastChecked
        {
            get => _lastChecked; set => SetProperty(ref _lastChecked, value);
        }

        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated
        {
            get => _lastUpdated; set => SetProperty(ref _lastUpdated, value);
        }

        public static UpdateSettings Default => new()
        {
            AutoUpdate = true,
            CheckUpdates = CheckUpdateOptions.ApplicationStart,
            IncludePreReleases = false,
            ShowReleaseNotes = false
        };
    }
}
