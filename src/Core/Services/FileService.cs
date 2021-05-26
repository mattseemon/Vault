using Newtonsoft.Json;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Exceptions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Seemon.Vault.Core.Services
{
    public class FileService : IFileService
    {

        public T Read<T>(string folderPath, string filename)
        {
            var path = Path.Combine(folderPath, filename);
            return Read<T>(path);
        }

        public T Read<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default;
        }

        public void Save<T>(string folderPath, string filename, T content)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);
            File.WriteAllText(Path.Combine(folderPath, filename), fileContent, Encoding.UTF8);
        }

        public void Delete(string folderPath, string filename)
        {
            if (filename != null && File.Exists(Path.Combine(folderPath, filename)))
            {
                File.Delete(Path.Combine(folderPath, filename));
            }
        }

        public string GetFileHash(IFileService.HashAlgorithms hashAlgorithm, string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("Invalid value for path", nameof(path));
            }

            using var hasher = HashAlgorithm.Create(hashAlgorithm.ToString());
            using var stream = File.OpenRead(path);

            var hash = hasher.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public bool VerifyFileIntegrity(string path, IFileService.HashAlgorithms hashAlgorithm, string hashValue)
        {
            var fileHash = GetFileHash(hashAlgorithm, path);

            return string.Compare(fileHash, hashValue, true) == 0;
        }

        public bool CopyDirectory(string source, string destination)
        {
            try
            {
                foreach (string path in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(path.Replace(source, destination));
                }

                foreach (string path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                {
                    File.Copy(path, path.Replace(source, destination), true);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new VaultException($"Copy file to destination {destination} failed.", ex);
            }
        }
    }
}
