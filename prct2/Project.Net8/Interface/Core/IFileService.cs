using Project.Net8.Models.Core;

namespace Project.Net8.Interface.Core;



public interface IFileService
{
    Task<FileShortModel> SaveFileAsync(IFormFile file);
    public Task<string> UploadFileCK(IFormFile file);

    Task<FileView> GetFileById(string id);
    
    
    Task<List<FileShortModel>> SaveMultiFileAsync(List<IFormFile> file);
    Task<bool> Delete(string id);


}
