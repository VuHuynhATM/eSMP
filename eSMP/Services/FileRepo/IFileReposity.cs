namespace eSMP.Services.FileRepo
{
    public interface IFileReposity
    {
        public Task<string> UploadFile(IFormFile formFile, string fileName);
        public Task<bool> DeleteFileASYNC(string fileName);
    }
}
