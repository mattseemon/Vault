using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Seemon.Vault.Core.Models
{
    public class WindowSettings : ObservableObject
    {
        private double _windowTop;
        private double _windowLeft;
        private double _windowHeight;
        private double _windowWidth;

        [JsonProperty("windowTop")]
        public double WindowTop
        {
            get => _windowTop; set => SetProperty(ref _windowTop, value);
        }

        [JsonProperty("windowLeft")]
        public double WindowLeft
        {
            get => _windowLeft; set => SetProperty(ref _windowLeft, value);
        }

        [JsonProperty("windowHeight")]
        public double WindowHeight
        {
            get => _windowHeight; set => SetProperty(ref _windowHeight, value);
        }

        [JsonProperty("windowWidth")]
        public double WindowWidth
        {
            get => _windowWidth; set => SetProperty(ref _windowWidth, value);
        }
    }
}
