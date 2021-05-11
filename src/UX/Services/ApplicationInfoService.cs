using Seemon.Vault.Core.Contracts.Services;
using System;
using System.Reflection;

namespace Seemon.Vault.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        private Assembly _assembly = Assembly.GetExecutingAssembly();

        public ApplicationInfoService() { }

        public string GetApplicationIdentifier() => $"Seemon.{GetTitle()}";

        public string GetAuthor() => GetAssemblyAttribute<AssemblyCompanyAttribute>(_assembly)?.Company ?? "Matt Seemon";

        public string GetCopyright() => GetAssemblyAttribute<AssemblyCopyrightAttribute>(_assembly)?.Copyright ?? "© Copyright 2021, Matt Seemon. All rights reserved.";

        public string GetDescription() => GetAssemblyAttribute<AssemblyDescriptionAttribute>(_assembly)?.Description ?? "A file system based secrets repository, powered by OpenPGP.";

        public string GetTitle() => GetAssemblyAttribute<AssemblyTitleAttribute>(_assembly)?.Title ?? "Vault";

        public string GetVersion() => GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(_assembly)?.InformationalVersion ?? string.Empty;

        public T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), true);

            if (attributes == null || attributes.Length == 0)
            {
                return null;
            }

            return (T)attributes[0];
        }
    }
}
