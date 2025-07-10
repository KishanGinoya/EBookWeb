namespace BookShoppingWebUI.Shared
{
    public interface IFileService
    {
        void DeleteFile(string fileName);
        Task<string> SaveFile(IFormFile file, string[] allowedExtentions);
    }
    public class FileService:IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> SaveFile(IFormFile file,string[] allowedExtentions)
        {
            var wwwRootPath=_webHostEnvironment.WebRootPath;
            var path= Path.Combine(wwwRootPath, "images");
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtentions.Contains(extension))
            {
                throw new Exception("File type is not allowed");
            }
            string fileName = Guid.NewGuid().ToString() + extension;
            string fileNameWithPath = Path.Combine(path, fileName);
            using var stream=new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }
        
        public void DeleteFile(string fileName)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(wwwRootPath, "images\\", fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", fileName);
            }
            File.Delete(path);
        }
    }
}
