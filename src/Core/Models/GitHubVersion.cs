using System;

namespace Seemon.Vault.Core.Models
{
    public class GitHubVersion : IComparable<GitHubVersion>
    {
        public GitHubVersion(string version)
        {
            Raw = version;

            if(version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                version = version.Substring(1);
            }

            var split = version.Split("-");
            if(split.Length > 1)
            {
                PreReleaseLabel = split[1];
                IsPreRelease = true;
            }

            split = split[0].Split(".");
            Major = int.Parse(split[0]);
            Minor = int.Parse(split[1]);
            Patch = int.Parse(split[2]);
        }

        public string Raw { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public bool IsPreRelease { get; set; } = false;

        public string PreReleaseLabel { get; set; } = string.Empty;

        public int CompareTo(GitHubVersion other)
        {
            if (Major.CompareTo(other.Major) != 0)
            {
                return Major.CompareTo(other.Major);
            }
            else if (Minor.CompareTo(other.Minor) != 0)
            {
                return Minor.CompareTo(other.Minor);
            }
            else if (Patch.CompareTo(other.Patch) != 0)
            {
                return Minor.CompareTo(other.Minor);
            }
            return 0;
        }

        public static bool operator <(GitHubVersion left, GitHubVersion right) => left.CompareTo(right) < 0;
        
        public static bool operator >(GitHubVersion left, GitHubVersion right) => left.CompareTo(right) > 0;

        public static bool operator >=(GitHubVersion left, GitHubVersion right) => left.CompareTo(right) >= 0;

        public static bool operator <=(GitHubVersion left, GitHubVersion right) => left.CompareTo(right) <= 0;

        public override string ToString() => $"{Major}.{Minor}.{Patch}-{PreReleaseLabel}";
    }
}
