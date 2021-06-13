using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using System.Collections;
using System.IO;
using System.Windows;

namespace Seemon.Vault.Core.Services
{
    public class DataService : IDataService
    {
        private readonly ILogger<IDataService> _logger;
        private readonly IFileService _fileService;
        private readonly IApplicationInfoService _appInfo;
        private readonly string _settingsFilename = "vault.settings.json";

        public DataService(IFileService fileService, IApplicationInfoService appInfo, ILogger<IDataService> logger)
        {
            _logger = logger;
            _fileService = fileService;
            _appInfo = appInfo;
        }

        public void PersistData()
        {
            _logger.LogInformation($"Saving settings to file : {_settingsFilename}");
            if (Application.Current.Properties is not null)
            {
                _fileService.Save(_appInfo.GetDataPath(), _settingsFilename, Application.Current.Properties);
            }
        }

        public void RestoreData()
        {
            _logger.LogInformation($"Loading settings from file : {_settingsFilename}");
            var properties = _fileService.Read<IDictionary>(Path.Combine(_appInfo.GetDataPath(), _settingsFilename));
            if (properties is not null)
            {
                foreach (DictionaryEntry property in properties)
                {
                    Application.Current.Properties.Add(property.Key, property.Value);
                }
            }
        }
    }
}
