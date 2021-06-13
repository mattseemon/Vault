using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Helpers.Extensions;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Seemon.Vault.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageService _pageService;
        private readonly ILogger<INavigationService> _logger;

        private Frame _frame;
        private object _lastParameterUsed;

        public event EventHandler<string> Navigated;

        public NavigationService(IPageService pageService, ILogger<INavigationService> logger)
        {
            _pageService = pageService;
            _logger = logger;
        }

        public bool CanGoBack => _frame.CanGoBack;

        public void Initialize(Frame shellFrame)
        {
            _logger.LogInformation($"Starting application navigation service.");
            if (_frame is null)
            {
                _frame = shellFrame;
                _frame.Navigated += OnNavigated;
            }
        }

        public void UnsubscribeNavigation()
        {
            _logger.LogInformation($"Stopping application navigation service.");
            if (_frame is not null)
            {
                _frame.Navigated -= OnNavigated;
                _frame = null;
            }
        }

        public void GoBack()
        {
            if (_frame.CanGoBack)
            {
                var vmBeforeNavigation = _frame.GetDataContext();
                _frame.GoBack();

                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigateFrom();
                }
            }
        }

        public bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false)
        {
            _logger.LogInformation($"Navigating to {pageKey}.");
            try
            {
                var pageType = _pageService.GetPageType(pageKey);

                if (_frame.Content?.GetType() != pageType || (parameter is not null && !parameter.Equals(_lastParameterUsed)))
                {
                    _frame.Tag = clearNavigation;
                    var page = _pageService.GetPage(pageKey);
                    var navigated = _frame.Navigate(page, parameter);

                    if (navigated)
                    {
                        _lastParameterUsed = parameter;
                        var dataContext = _frame.GetDataContext();
                        if (dataContext is INavigationAware navigationAware)
                        {
                            navigationAware.OnNavigateFrom();
                        }
                    }
                    return navigated;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);
            }
            return false;
        }

        public void CleanNavigation() => _frame.CleanNavigation();

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                var clearNavigation = (bool)frame.Tag;
                if (clearNavigation)
                {
                    frame.CleanNavigation();
                }

                var dataContext = frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigateTo(e.ExtraData);
                }

                Navigated?.Invoke(sender, dataContext.GetType().FullName);
            }
        }
    }
}
