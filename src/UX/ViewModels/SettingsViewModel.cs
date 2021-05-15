using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers;
using Seemon.Vault.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Seemon.Vault.ViewModels
{
    public class SettingsViewModel : ViewModelBase, INavigationAware
    {
        public class AccentColorData
        {
            public string Name { get; set; }
            public Brush ColorBrush { get; set; }
        }

        private readonly ISettingsService _settingsService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly IWindowManagerService _windowManagerService;
        private readonly ITaskbarIconService _taskbarIconService;
        private readonly ISystemService _systemService;

        private ApplicationTheme _theme;
        private SystemSettings _systemSettings;
        private ClipboardSettings _clipboardSettings;
        private UpdateSettings _updateSettings;
        private PGPSettings _pgpSettings;
        private GitSettings _gitSettings;
        private PasswordGeneratorSettings _passwordGeneratorSettings;
        private SecuritySettings _securitySettings;
        private ObservableCollection<Profile> _profiles;
        private Profile _selectedProfile;

        private ICommand _selectionChangedCommand;
        private ICommand _checkedCommand;

        private ICommand _newProfileCommand;
        private ICommand _editProfileCommand;
        private ICommand _deleteProfilesCommand;
        private ICommand _defaultProfileCommand;

        public SettingsViewModel(ISettingsService settingsService, IThemeSelectorService themeSelectorService, 
            IWindowManagerService windowManagerService, ITaskbarIconService taskbarIconService, 
            ISystemService systemService)
        {
            _settingsService = settingsService;
            _themeSelectorService = themeSelectorService;
            _windowManagerService = windowManagerService;
            _taskbarIconService = taskbarIconService;
            _systemService = systemService;
        }

        public ICommand SelectionChangedCommand => _selectionChangedCommand ??= RegisterCommand<string>(OnSelectionChanged);

        public ICommand CheckedCommand => _checkedCommand ??= RegisterCommand<string>(OnChecked);

        public ICommand NewProfileCommand => _newProfileCommand ??= RegisterCommand(OnNewProfile);

        public ICommand EditProfileCommand => _editProfileCommand ??= RegisterCommand(OnEditProfile, CanEditProfile);

        public ICommand DeleteProfilesCommand => _deleteProfilesCommand ??= RegisterCommand(OnDeleteProfiles, CanDeleteProfiles);

        public ICommand DefaultProfileCommand => _defaultProfileCommand ??= RegisterCommand(OnDefaultProfile, CanDefaultProfile);

        public List<AccentColorData> AccentColors { get; set; }

        public ApplicationTheme Theme
        {
            get => _theme; set => SetProperty(ref _theme, value);
        }

        public SystemSettings System
        {
            get => _systemSettings; set => SetProperty(ref _systemSettings, value);
        }

        public ClipboardSettings Clipboard
        {
            get => _clipboardSettings; set => SetProperty(ref _clipboardSettings, value);
        }

        public UpdateSettings Updates
        {
            get => _updateSettings; set => SetProperty(ref _updateSettings, value);
        }

        public PGPSettings PGP
        {
            get => _pgpSettings; set => SetProperty(ref _pgpSettings, value);
        }

        public GitSettings Git
        {
            get => _gitSettings; set => SetProperty(ref _gitSettings, value);
        }

        public PasswordGeneratorSettings PasswordGenerator
        {
            get => _passwordGeneratorSettings; set => SetProperty(ref _passwordGeneratorSettings, value);
        }

        public SecuritySettings Security
        {
            get => _securitySettings; set => SetProperty(ref _securitySettings, value);
        }

        public ObservableCollection<Profile> Profiles
        {
            get => _profiles; set => SetProperty(ref _profiles, value);
        }

        public Profile SelectedProfile
        {
            get => _selectedProfile; set => SetProperty(ref _selectedProfile, value);
        }

        public IEnumerable<Profile> SelectedProfiles => _profiles.Where(x => x.IsSelected);

        public void OnNavigateTo(object parameter)
        {
            AccentColors = new List<AccentColorData>();
            AccentColors.Add(new AccentColorData { Name = "System", ColorBrush = SystemParameters.WindowGlassBrush });
            AccentColors.AddRange(ThemeManager.Current.Themes
                .GroupBy(x => x.ColorScheme)
                .OrderBy(a => a.Key)
                .Select(a => new AccentColorData { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                .ToList());

            Theme = _settingsService.Get(Constants.SETTINGS_APPLICATION_THEME, ApplicationTheme.Default);
            System = _settingsService.Get(Constants.SETTINGS_SYSTEM, SystemSettings.Default);
            Clipboard = _settingsService.Get(Constants.SETTINGS_CLIPBOARD, ClipboardSettings.Default);
            Updates = _settingsService.Get(Constants.SETTINGS_UDPATES, UpdateSettings.Default);
            PGP = _settingsService.Get(Constants.SETTINGS_PGP, PGPSettings.Default);
            Git = _settingsService.Get(Constants.SETTINGS_GIT, GitSettings.Default);
            PasswordGenerator = _settingsService.Get(Constants.SETTINGS_PASSWORD_GENERATOR, PasswordGeneratorSettings.Default);
            Security = _settingsService.Get(Constants.SETTINGS_SECURITY, SecuritySettings.Default);
            Profiles = _settingsService.Get(Constants.SETTINGS_PROFILES, new ObservableCollection<Profile>());
        }

        public void OnNavigateFrom() { }

        private void OnSelectionChanged(string parameter)
        {
            switch (parameter)
            {
                case "Theme":
                    _themeSelectorService.SetTheme(Theme);
                    break;
            }
            RaiseCommandsCanExecute();
        }

        private void OnChecked(string parameter)
        {
            switch (parameter)
            {
                case "TaskbarIcon":
                    if (System.ShowVaultInNotificationArea)
                        _taskbarIconService.Initialize();
                    else
                        _taskbarIconService.Destroy();
                    break;
                case "StartWithWindows":
                    _systemService.SetAutoStartWithWindows(System.StartWithWindows);
                    break;
                case "AlwaysOnTop":
                    _windowManagerService.MainWindow.Topmost = System.AlwaysOnTop;
                    break;
            }
        }

        private void OnNewProfile()
        {
            IWindow window = App.GetService<ProfileWindow>();
            var response = window.ShowDialog(App.GetService<IShellWindow>());
            if (response.HasValue && response.Value)
            {
                ProfileViewModel vm = window.ViewModel as ProfileViewModel;

                var profile = new Profile
                {
                    Name = vm.Name,
                    Location = vm.Location
                };

                Profiles.Add(profile);
            }
        }

        private bool CanEditProfile() => SelectedProfiles.Count() == 1;

        private void OnEditProfile()
        {
            IWindow window = App.GetService<ProfileWindow>();

            ProfileViewModel vm = window.ViewModel as ProfileViewModel;
            vm.SetModel(SelectedProfile);

            var response = window.ShowDialog(App.GetService<IShellWindow>());

            if (response.HasValue && response.Value)
            {
                SelectedProfile.Name = vm.Name;
                SelectedProfile.Location = vm.Location;
            }
        }

        private bool CanDeleteProfiles() => SelectedProfiles.Count() > 0;

        private async void OnDeleteProfiles()
        {
            IWindow window = _windowManagerService.GetWindow(PageKey);

            var options = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No", ColorScheme = MetroDialogColorScheme.Inverted };
            var response = await ((WindowBase)window).ShowMessageAsync("Delete profiles!", "Are you sure you want to delete the selected profiles?", MessageDialogStyle.AffirmativeAndNegative, options);

            if (response == MessageDialogResult.Affirmative)
            {
                var profiles = SelectedProfiles.ToList();
                foreach (var profile in profiles)
                {
                    Profiles.Remove(profile);
                }
            }
        }

        private bool CanDefaultProfile() => SelectedProfiles.Count() == 1;

        private void OnDefaultProfile()
        {
            foreach (var profile in Profiles)
            {
                profile.IsDefault = (profile == SelectedProfile);
            }
        }
    }
}
