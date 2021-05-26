using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Services
{
    public class HttpService : IHttpService
    {
        private HttpClient _client;
        private readonly ILogger<IHttpService> _logger;

        public HttpService(ILogger<IHttpService> logger)
        {
            _logger = logger;

            _logger.LogInformation("Initialize HTTP Service");

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            handler.UseCookies = false;

            _client = new HttpClient(handler, true);
            _client.DefaultRequestHeaders.Add("User-Agent", "Seemon.Vault (github.com/mattseemon/Vault)");
        }

        public async Task<string> DownloadFileAsync(string url, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Downloading file: {url}");
            string path = Path.Join(Path.GetTempPath(), url[(url.LastIndexOf("/") + 1)..]);

            var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(60));

            try
            {
                using var contentStream = await response.Content.ReadAsStreamAsync(cts.Token);
                var totalBytesRead = 0L;
                var readCount = 0L;
                var buffer = new byte[8192];
                var moreToRead = true;

                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token);

                    if (bytesRead == 0)
                    {
                        moreToRead = false;
                        ReportProgress(progress, totalBytes, totalBytesRead);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cts.Token);
                    totalBytesRead += bytesRead;
                    readCount++;

                    if (readCount % 100 == 0)
                    {
                        ReportProgress(progress, totalBytes, totalBytesRead);
                    }

                } while (moreToRead);

                _logger.LogInformation("Download completed: {0}", new { path, size = totalBytes });
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException();
            }

            return path;
        }

        private void ReportProgress(IProgress<double> progress, long? totalSize, long totalSizeRead)
        {
            if (totalSize.HasValue)
            {
                var percentage = Math.Round((double)totalSizeRead / totalSize.Value * 100, 2);
                progress?.Report(percentage);
            }
        }

        public async Task<string> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public HttpClient GetClient() => _client;
    }
}
