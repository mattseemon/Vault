using Seemon.Vault.Core.Contracts.Services;
using System;
using System.Reflection;

namespace Seemon.Vault.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        private Assembly _assembly = Assembly.GetExecutingAssembly();

        public string GetApplicationIdentifier() => $"Seemon.{GetTitle()}";

        public string GetAuthor() => GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company ?? "Matt Seemon";

        public string GetCopyright() => GetAssemblyAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "© Copyright 2021, Matt Seemon. All rights reserved.";

        public string GetDescription() => GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description ?? "A file system based secrets repository, powered by OpenPGP.";

        public string GetTitle() => GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title ?? "Vault";

        public string GetVersion() => GetAssemblyAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;

        public T GetAssemblyAttribute<T>() where T : Attribute
        {
            object[] attributes = _assembly.GetCustomAttributes(typeof(T), true);

            return attributes == null || attributes.Length == 0 ? null : (T)attributes[0];
        }
    }
}
