using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Seemon.Vault.Core.Models.Updater
{
    public class GitHubRelease
    {
        private JObject _releaseObject;
        private string _url;
        private string _name;
        private string _contents;
        private string _tag;
        private GitHubVersion _version;
        private List<GitHubAsset> _assets;

        public GitHubRelease(string releaseJson)
        {
            _releaseObject = JObject.Parse(releaseJson);
            _tag = _releaseObject["tag_name"].ToString();
            _contents = _releaseObject["body"].ToString();

            _name = _releaseObject["name"].ToString();
            _url = _releaseObject["html_url"].ToString();
            _version = new GitHubVersion(_tag);

            var assets = _releaseObject.SelectTokens("$.assets[*]");

            _assets = new List<GitHubAsset>();
            foreach (var asset in assets)
            {
                _assets.Add(new GitHubAsset(asset.ToString()));
            }
        }

        public string Url => _url;

        public string Name => _name;

        public string Contents => _contents;

        public GitHubVersion Version => _version;

        public IReadOnlyList<GitHubAsset> Assets => _assets;
    }
}
