namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IFileService
    {
        public enum HashAlgorithms
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        T Read<T>(string folderPath, string filename);

        T Read<T>(string filePath);

        void Save<T>(string folderPath, string filename, T content);

        void Delete(string folderPath, string filename);

        string GetFileHash(HashAlgorithms hashAlgorithm, string path);

        bool VerifyFileIntegrity(string path, HashAlgorithms hashAlgorithm, string hashValue);

        bool CopyDirectory(string source, string destination);
    }
}
