namespace ZndAlhadedStore.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        void DeleteFile(string fileUrl); 
    }
}
