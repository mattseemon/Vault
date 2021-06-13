using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace Seemon.Vault.Core.Models.Settings
{
    public class PGPSettings : ObservableObject
    {
        public enum FileFormats
        {
            [Description("Binary")]
            Binary,
            [Description("ASCII Armor")]
            AsciiArmor
        }

        private FileFormats _encryptFormat;
        private FileFormats _exportFormat;
        private bool _cachePassword;
        private bool _autoClear;
        private int _autoClearDuration;
        private string _lastImportPath;
        private string _lastExportPath;

        [JsonProperty("format")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FileFormats EncryptFormat
        {
            get => _encryptFormat; set => SetProperty(ref _encryptFormat, value);
        }

        [JsonProperty("exportFormat")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FileFormats ExportFormat
        {
            get => _exportFormat; set => SetProperty(ref _exportFormat, value);
        }

        [JsonProperty("cachePassword")]
        public bool CachePassword
        {
            get => _cachePassword; set => SetProperty(ref _cachePassword, value);
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

        [JsonProperty("lastImportPath")]
        public string LastImportPath
        {
            get => _lastImportPath; set => SetProperty(ref _lastImportPath, value);
        }

        [JsonProperty("lastExportPath")]
        public string LastExportPath
        {
            get => _lastExportPath; set => SetProperty(ref _lastExportPath, value);
        }

        public static PGPSettings Default => new()
        {
            EncryptFormat = FileFormats.Binary,
            ExportFormat = FileFormats.AsciiArmor,
            CachePassword = true,
            AutoClear = true,
            AutoClearDuration = 10,
            LastImportPath = string.Empty,
            LastExportPath = string.Empty
        };
    }
}
