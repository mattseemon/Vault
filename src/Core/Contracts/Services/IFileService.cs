namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IFileService
    {
        T Read<T>(string folderPath, string filename);

        void Save<T>(string folderPath, string filename, T content);

        void Delete(string folderPath, string filename);
    }
}
