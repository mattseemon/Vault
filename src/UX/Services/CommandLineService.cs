using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;

namespace Seemon.Vault.Services
{
    public class CommandLineService : ICommandLineService
    {
        private readonly ILogger<ICommandLineService> _logger;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IUpdateService _updateService;
        private readonly IApplicationInfoService _applicationInfoService;

        public CommandLineService(ILogger<ICommandLineService> logger, IConfiguration configuration, 
            INotificationService notificationService, IUpdateService updateService, 
            IApplicationInfoService applicationInfoService)
        {
            _logger = logger;
            _configuration = configuration;
            _notificationService = notificationService;
            _updateService = updateService;
            _applicationInfoService = applicationInfoService;
        }
        public void ProcessCommandLineArguments()
        {

            var updatePath = _configuration.GetValue<string>("UpdatePath");
            if (!string.IsNullOrEmpty(updatePath))
            {
                _updateService.UpdateAndRestart(updatePath);
            }

            var updateStatus = _configuration.GetValue<string>("UpdateStatus");
            if (!string.IsNullOrEmpty(updateStatus))
            {
                string updateMessage;
                if (bool.Parse(updateStatus))
                {
                    updateMessage = $"Application updated to version {_applicationInfoService.GetVersion()}.";
                    _logger.LogInformation(updateMessage);
                    _notificationService.ShowMessage(Models.NotificationType.Success, updateMessage, "Application Update - Success");
                }
                else
                {
                    updateMessage = $"Reverted back to version {_applicationInfoService.GetVersion()}.\nCheck the logs to see what went wrong and try again later.";
                    _logger.LogInformation(updateMessage);
                    _notificationService.ShowMessage(Models.NotificationType.Warning, updateMessage, "Application Update - Failed");
                }
            }
        }
    }
}
