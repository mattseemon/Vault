using Newtonsoft.Json;
using Seemon.Vault.Core.Contracts.Services;
using System.IO;
using System.Text;

namespace Seemon.Vault.Core.Services
{
    public class FileService : IFileService
    {
        public T Read<T>(string folderPath, string filename)
        {
            var path = Path.Combine(folderPath, filename);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
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
    }
}
