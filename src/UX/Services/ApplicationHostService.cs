using Microsoft.Extensions.Hosting;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly IDataService _dataService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly INavigationService _navigationService;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private IShellWindow _shellWindow;
        private bool _isInitialized;

        public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers, IDataService dataService, IThemeSelectorService themeSelectorService, INavigationService navigationService)
        {
            _serviceProvider = serviceProvider;
            _activationHandlers = activationHandlers;
            _dataService = dataService;
            _themeSelectorService = themeSelectorService;
            _navigationService = navigationService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeAsync();
            await HandleActivationAsync();
            await StartupAsync();

            _isInitialized = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _dataService.PersistData();

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

            if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
            {
                _shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
                _navigationService.Initialize(_shellWindow.GetNavigationFrame());
                _shellWindow.ShowWindow();
                _navigationService.NavigateTo(typeof(WelcomeViewModel).FullName);
                await Task.CompletedTask;
            }
        }
    }
}
