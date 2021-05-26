using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace Seemon.Vault.Core.Models
{
    public class SecuritySettings : ObservableObject
    {
        public enum SelfEraseModes
        {
            [Description("OpenPGP key store only")]
            OpenPGPOnly,
            [Description("OpenPGP key store and vault files")]
            OpenPGPAndVault
        }

        private bool _enablePasswordRotation = false;
        private int _passwordRotationDuration = 30;
        private int _minimumUniquePasswordCount = 5;

        private bool _selfErase = false;
        private int _selfEraseOnFailureCount = 5;
        private SelfEraseModes _selfEraseMode = SelfEraseModes.OpenPGPOnly;

        [JsonProperty("enablePasswordRotation")]
        public bool EnablePasswordRotation
        {
            get => _enablePasswordRotation; set => SetProperty(ref _enablePasswordRotation, value);
        }

        [JsonProperty("passwordRotationDuration")]
        public int PasswordRotationDuration
        {
            get => _passwordRotationDuration; set => SetProperty(ref _passwordRotationDuration, value);
        }

        [JsonProperty("minimumUniquePasswordCount")]
        public int MinimumUniquePasswordCount
        {
            get => _minimumUniquePasswordCount; set => SetProperty(ref _minimumUniquePasswordCount, value);
        }

        [JsonProperty("selfErase")]
        public bool SelfErase
        {
            get => _selfErase; set => SetProperty(ref _selfErase, value);
        }

        [JsonProperty("selfEraseOnFailureCount")]
        public int SelfEraseOnFailureCount
        {
            get => _selfEraseOnFailureCount; set => SetProperty(ref _selfEraseOnFailureCount, value);
        }

        [JsonProperty("selfEraseMode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SelfEraseModes SelfEraseMode
        {
            get => _selfEraseMode; set => SetProperty(ref _selfEraseMode, value);
        }

        public static SecuritySettings Default => new()
        {
            EnablePasswordRotation = false,
            PasswordRotationDuration = 30,
            MinimumUniquePasswordCount = 5,
            SelfErase = false,
            SelfEraseOnFailureCount = 5,
            SelfEraseMode = SelfEraseModes.OpenPGPOnly
        };
    }
}
