using Microsoft.Extensions.Options;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Models;
using System;
using System.Collections;
using System.IO;

namespace Seemon.Vault.Services
{
    public class DataService : IDataService
    {
        private readonly IFileService _fileService;
        private readonly ApplicationConfig _appConfig;
        private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public DataService(IFileService fileService, IOptions<ApplicationConfig> appConfig)
        {
            _fileService = fileService;
            _appConfig = appConfig.Value;
        }

        public void PersistData()
        {
            if (App.Current.Properties != null)
            {
                var foldePath = Path.Combine(_localAppData, _appConfig.ConfigPath);
                var filename = _appConfig.ConfigFile;
                _fileService.Save(foldePath, filename, App.Current.Properties);
            }
        }

        public void RestoreData()
        {
            var folderPath = Path.Combine(_localAppData, _appConfig.ConfigPath);
            var filename = _appConfig.ConfigFile;
            var properties = _fileService.Read<IDictionary>(folderPath, filename);
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
