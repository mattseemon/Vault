using Newtonsoft.Json.Linq;

namespace Seemon.Vault.Core.Models.Updater
{
    public class GitHubAsset
    {
        private JObject _asset;
        private string _name;
        private string _downloadUrl;
        private uint _size;

        public GitHubAsset(string assetJson)
        {
            _asset = JObject.Parse(assetJson);

            _name = _asset["name"].ToString();
            _downloadUrl = _asset["browser_download_url"].ToString();
            _size = uint.Parse(_asset["size"].ToString());
        }

        public string Name => _name;
        public string DownloadUrl => _downloadUrl;
        public uint Size => _size;
    }
}
