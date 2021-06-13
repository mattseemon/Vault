using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Seemon.Vault.Core.Models.Settings
{
    public class GitSettings : ObservableObject
    {
        private bool _useGit = false;
        private bool _autoAddGpgIdFiles = false;
        private bool _autoPush = false;
        private bool _autoPull = false;

        [JsonProperty("useGit")]
        public bool UseGit
        {
            get => _useGit; set => SetProperty(ref _useGit, value);
        }

        [JsonProperty("autoAddGpgIdFiles")]
        public bool AutoAddGpgIdFiles
        {
            get => _autoAddGpgIdFiles; set => SetProperty(ref _autoAddGpgIdFiles, value);
        }

        [JsonProperty("autoPush")]
        public bool AutoPush
        {
            get => _autoPush; set => SetProperty(ref _autoPush, value);
        }

        [JsonProperty("autoPull")]
        public bool AutoPull
        {
            get => _autoPull; set => SetProperty(ref _autoPull, value);
        }

        public static GitSettings Default => new()
        {
            UseGit = false,
            AutoAddGpgIdFiles = false,
            AutoPush = false,
            AutoPull = false
        };
    }
}
