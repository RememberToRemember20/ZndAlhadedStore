using ZndAlhadedStore.Interfaces;

namespace ZndAlhadedStore.Implment
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadsFolder;

        public FileStorageService(IWebHostEnvironment env)
        {
            // استخدم WebRootPath، وإذا كان null، قم بإنشاء المسار يدوياً داخل مسار المشروع
            var webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            _uploadsFolder = Path.Combine(webRootPath, "images");
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }
        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_uploadsFolder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
