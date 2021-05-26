using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using System.Collections;

namespace Seemon.Vault.Services
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
            if (App.Current.Properties != null)
            {
                _fileService.Save(_appInfo.GetDataPath(), _settingsFilename, App.Current.Properties);
            }
        }

        public void RestoreData()
        {
            _logger.LogInformation($"Loading settings from file : {_settingsFilename}");
            var properties = _fileService.Read<IDictionary>(_appInfo.GetDataPath(), _settingsFilename);
            if (properties != null)
            {
                foreach (DictionaryEntry property in properties)
                {
                    App.Current.Properties.Add(property.Key, property.Value);
                }
            }
        }
    }
}
