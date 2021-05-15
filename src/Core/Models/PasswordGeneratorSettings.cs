using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Seemon.Vault.Core.Models
{
    public class PasswordGeneratorSettings : ObservableObject
    {
        private int _length;
        private bool _includeUppercase;
        private bool _includeLowercase;
        private bool _includeNumerals;
        private bool _includeSpace;
        private bool _includeSpecial;
        private string _excludeCharacters;

        [JsonProperty("length")]
        public int Length
        {
            get => _length; set => SetProperty(ref _length, value);
        }

        [JsonProperty("includeUppercase")]
        public bool IncludeUppercase
        {
            get => _includeUppercase; set => SetProperty(ref _includeUppercase, value);
        }

        [JsonProperty("includeLowercase")]
        public bool IncludeLowercase
        {
            get => _includeLowercase; set => SetProperty(ref _includeLowercase, value);
        }

        [JsonProperty("includeNumerals")]
        public bool IncludeNumerals
        {
            get => _includeNumerals; set => SetProperty(ref _includeNumerals, value);
        }

        [JsonProperty("includeSpace")]
        public bool IncludeSpace
        {
            get => _includeSpace; set => SetProperty(ref _includeSpace, value);
        }

        [JsonProperty("includeSpecial")]
        public bool IncludeSpecial
        {
            get => _includeSpecial; set => SetProperty(ref _includeSpecial, value);
        }

        [JsonProperty("excludeCharacters")]
        public string ExcludeCharacters
        {
            get => _excludeCharacters; set => SetProperty(ref _excludeCharacters, value);
        }

        public static PasswordGeneratorSettings Default => new()
        {
            Length = 8,
            IncludeUppercase = true,
            IncludeLowercase = true,
            IncludeNumerals = true,
            IncludeSpace = true,
            IncludeSpecial = true,
            ExcludeCharacters = string.Empty
        };
    }
}
