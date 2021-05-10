using System;
using System.Windows.Controls;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);

        Page GetPage(string key);
    }
}
