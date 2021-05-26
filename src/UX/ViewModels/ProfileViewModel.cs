using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers;
using Seemon.Vault.Helpers.Validators;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly ISystemService _systemService;
        private readonly IWindowManagerService _windowManagerService;

        private string _name;
        private string _location;

        private ICommand _submitCommand;
        private ICommand _browseCommand;

        public ProfileViewModel(ISystemService systemService, IWindowManagerService windowManagerService)
        {
            _systemService = systemService;
            _windowManagerService = windowManagerService;
        }

        public ICommand SubmitCommand => _submitCommand ??= RegisterCommand(OnSubmit, CanSubmit);

        public ICommand BrowseCommad => _browseCommand ??= RegisterCommand(OnBrowse);

        [Required]
        public string Name
        {
            get => _name; set => SetProperty(ref _name, value, true);
        }

        [Required]
        [PathValidator]
        public string Location
        {
            get => _location;
            set
            {
                SetProperty(ref _location, value, true);
                if (string.IsNullOrEmpty(_name))
                {
                    Name = value;
                }
            }
        }

        public override void SetModel(object model)
        {
            if (model is Profile profile)
            {
                Name = profile.Name;
                Location = profile.Location;
            }
        }

        private bool CanSubmit() => !HasErrors && !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_location);

        private void OnSubmit()
        {
            IWindow window = _windowManagerService.GetWindow(PageKey);

            if (window != null)
            {
                window.CloseDialog(true);
            }
        }

        private void OnBrowse()
        {
            var location = string.IsNullOrEmpty(Location) ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) : Location;
            location = _systemService.ShowFolderDialog("Select root path to vault", location);
            if (!string.IsNullOrEmpty(location))
                Location = location;
        }
    }
}
