using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Activation;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Seemon.Vault.Services
{
    public class ApplicationHostService : IHostedService
    {
        private readonly ILogger<IHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDataService _dataService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly INavigationService _navigationService;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly ITaskbarIconService _taskbarIconService;
        private readonly IWindowManagerService _windowManagerService;
        private readonly ICommandLineService _commandLineService;

        private IShellWindow _shellWindow;
        private bool _isInitialized;

        public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers,
            IDataService dataService, IThemeSelectorService themeSelectorService, INavigationService navigationService,
            ITaskbarIconService taskbarIconService, IWindowManagerService windowManagerService,
            ILogger<IHostedService> logger, ICommandLineService commandLineService)
        {
            _serviceProvider = serviceProvider;
            _activationHandlers = activationHandlers;
            _dataService = dataService;
            _themeSelectorService = themeSelectorService;
            _navigationService = navigationService;
            _taskbarIconService = taskbarIconService;
            _windowManagerService = windowManagerService;
            _logger = logger;
            _commandLineService = commandLineService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Application services are being initialized");

            await InitializeAsync();
            await HandleActivationAsync();
            await StartupAsync();

            _isInitialized = true;

            _logger.LogInformation($"Application services initialized");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Application services are being stopped");

            _taskbarIconService.Destroy();
            _dataService.PersistData();

            _logger.LogInformation($"Application shutdown complete.");
            await Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                _dataService.RestoreData();
                _themeSelectorService.InitializeTheme();

                await Task.CompletedTask;
            }
        }

        private async Task StartupAsync()
        {
            if (!_isInitialized)
            {
                _taskbarIconService.Initialize();
                _windowManagerService.RestoreWindowSettings();

                await Task.CompletedTask;
            }
        }

        private async Task HandleActivationAsync()
        {
            var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle());
            if (activationHandler != null)
            {
                await activationHandler.HandleAsync();
            }

            await Task.CompletedTask;

            if (!App.Current.Windows.OfType<IShellWindow>().Any())
            {
                _shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
                _navigationService.Initialize(_shellWindow.GetNavigationFrame());
                _shellWindow.Show();
                _navigationService.NavigateTo(typeof(WelcomeViewModel).FullName);
            }

            _commandLineService.ProcessCommandLineArguments();

            await Task.CompletedTask;
        }
    }
}
