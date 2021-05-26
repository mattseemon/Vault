using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IHttpService
    {
        HttpClient GetClient();

        Task<string> GetAsync(string url, CancellationToken cancellationToken);

        Task<string> DownloadFileAsync(string url, IProgress<double> progress = null, CancellationToken cancellationToken = default);
    }
}
