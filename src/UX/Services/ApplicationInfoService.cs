using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Models.Updater;
using System;
using System.IO;
using System.Reflection;

namespace Seemon.Vault.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        private Assembly _assembly = Assembly.GetExecutingAssembly();
        private string _executablePath;
        private string _dataPath;

        private string _title;
        private string _company;
        private string _copyright;
        private string _description;
        private string _version;
        private bool _isPreRelease;

        public ApplicationInfoService()
        {
            _executablePath = Path.GetDirectoryName(_assembly.Location);
            _title = GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title ?? "Vault";
            _company = GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company ?? "Matt Seemon";
            _copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "© Copyright 2021, Matt Seemon. All rights reserved.";
            _description = GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description ?? "A file system based secrets repository, powered by OpenPGP.";

            var version = new GitHubVersion(GetAssemblyAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
            _version = version.ToString();
            _isPreRelease = version.IsPreRelease;

            var path = Path.Combine(_executablePath, "data");

            _dataPath = Directory.Exists(path)
                ? path
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), GetApplicationIdentifier());

            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public string GetApplicationIdentifier() => $"Seemon.{GetTitle()}";

        public string GetAuthor() => _company;

        public string GetCopyright() => _copyright;

        public string GetDescription() => _description;

        public string GetTitle() => _title;

        public string GetVersion() => _version;

        public bool GetIsPreRelease() => _isPreRelease;

        public string GetDataPath() => _dataPath;

        public string GetLogPath() => Path.Combine(_dataPath, "logs");

        public string GetKeyStorePath() => Path.Combine(_dataPath, "keystore.db");

        public string GetApplicationExecutablePath() => _executablePath;

        private T GetAssemblyAttribute<T>() where T : Attribute
        {
            var attributes = _assembly.GetCustomAttributes(typeof(T), true);

            return attributes is null || attributes.Length == 0 ? null : (T)attributes[0];
        }
    }
}
