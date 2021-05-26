using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Seemon.Vault.Core.Models
{
    public class Profile : ObservableObject
    {
        private string _name;
        private string _location;
        private bool _isDefault;

        [JsonProperty("name")]
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? _location : _name; set => SetProperty(ref _name, value);
        }

        [JsonProperty("location")]
        public string Location
        {
            get => _location; set => SetProperty(ref _location, value);
        }

        [JsonProperty("isDefault")]
        public bool IsDefault
        {
            get => _isDefault; set => SetProperty(ref _isDefault, value);
        }

        [JsonIgnore]
        public bool IsSelected { get; set; }

        public static Profile Default => new()
        {
            Name = string.Empty,
            Location = string.Empty,
            IsDefault = false,
            IsSelected = false
        };
    }
}
