using System.IO;
using System.Net;
using DTC.DefaultRepository.Constants;
using DTC.DefaultRepository.Exceptions;
using DTC.DefaultRepository.Helpers;
using DTC.MongoDB;
using Minio.DataModel.Args;
using MongoDB.Driver;
using Project.Net8.Installers;
using Project.Net8.Interface.Core;
using Project.Net8.Models.Appsettings;
using Project.Net8.Models.Core;
using Project.Net8.Service.Base;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace Project.Net8.Service.Core
{
    public class FileService :  BaseService, IFileService
    {
    private DataContext _context;
    private BaseMongoDb<FileModel, string> BaseMongoDb;
    private readonly IWebHostEnvironment _hostingEnvironment;
    IMongoCollection<FileModel> _collection;
    
    public FileService(
        DataContext context,
        IWebHostEnvironment hostingEnvironment,
        IHttpContextAccessor contextAccessor) :
        base(context, contextAccessor)
    {
        _context = context;
        _hostingEnvironment = hostingEnvironment;
        BaseMongoDb = new BaseMongoDb<FileModel, string>(_context.FILES);
        _collection = context.FILES;
    }
    
        public async Task<FileShortModel> SaveFileAsync(IFormFile file )
        {
            
            if (file == null || (file != null && file.Length == 0))
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("File tải lên đang bị rỗng");
            }
            var fileName = Path.GetFileName(file.FileName);
            FileInfo fileInfo = new FileInfo(fileName);
            var extFile = fileInfo.Extension;
            if (fileName.Length > 100)
            {
                throw new Exception("Tên tệp tin quá dài.");
            }
            var dateTime = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ssss");
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, "files/", dateTime);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!NKTM.Constants.DefaultFile.FILES.Contains(extFile))
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("File không thể cập nhật vào hệ thống");
            
            
            var newFileName = Guid.NewGuid().ToString() + extFile;
            var relativePath = Path.Combine("", dateTime, newFileName);
            var filePath = Path.Combine("files/", relativePath);
            using (var strem = File.Create(filePath))
            {
                file.CopyTo(strem);
            }
            var entity = new FileModel();
            entity.FileName = fileName;
            entity.SaveName = newFileName;
            entity.Path = filePath;
            entity.Size = file.Length;
            entity.Ext = extFile;

            var result = await BaseMongoDb.CreateAsync(entity);
            
            if (result.Entity.Id == default || !result.Success)
            {
                throw new ResponseMessageException()
                    .WithException(DefaultCode.CREATE_FAILURE);
            }
            var fileshort = new FileShortModel();
            fileshort.FileId = entity.Id;
            fileshort.FileName = entity.FileName;
            fileshort.Ext = entity.Ext;
            fileshort.Path = entity.Path;
            return fileshort;
        }

        public async Task<FileView> GetFileById(string id)
        {
            HttpResponseMessage result = null;
            var file = new FileView();
        //    MemoryStream memory = new MemoryStream();
            var pathFile = "";
            try
            {
                var data = _context.FILES.Find(x => !x.IsDeleted && x.Id == id).FirstOrDefault();
                if (data == null) 
                    throw new ResponseMessageException()
                        .WithException(DefaultCode.DATA_NOT_FOUND);
                
                var localFilePath = Path.Combine(data.Path);
                pathFile = Path.Combine(_hostingEnvironment.ContentRootPath, localFilePath);
                  file.FileName = data.FileName;
                    var info = System.IO.File.GetAttributes(pathFile);
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StreamContent(new FileStream(pathFile, FileMode.Open, FileAccess.Read));
                    result.Content.Headers.Add("x-filename", file.FileName );
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                    result.Content.Headers.ContentDisposition.FileName = file.FileName ;
                    using (FileStream stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                    {
                        await stream.CopyToAsync(file.data);
                    }

                    file.data.Position = 0;
                    return file;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }

            return null;
        }

        public Task<List<FileShortModel>> SaveMultiFileAsync(List<IFormFile> file)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Delete(string id)
        {
            if (id == default)
                throw new ResponseMessageException().WithCode(1)
                    .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
            var entity = _collection.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();

            if (entity == default)
                throw new ResponseMessageException().WithCode(2)
                    .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        
            entity.IsDeleted = true;
            entity.FileName = null;
            entity.SaveName = null;
            entity.Ext = null;
            entity.Path = null;
         


            var result = await BaseMongoDb.UpdateAsync(entity);
            if (!result.Success)
                return false;
            return true;
        }
        public async Task<FileShortModel> SaveFileAsyncMinio(string fileName, string saveName, string path, string pathFolder, long fileSize, string fileExt, string foreignKey)
        {
            var entity = new FileModel();
            entity.FileName = fileName;
            entity.SaveName = saveName;
            entity.Path = path;
            entity.PathFolder = pathFolder;
            entity.Size = fileSize;
            entity.Ext = fileExt;
            entity.CreatedAt = DateTime.Now;
            entity.IsDeleted = false;
          //  entity.Key = foreignKey;

            var result = await BaseMongoDb.CreateAsync(entity);

            if (result.Entity.Id == default || !result.Success)
                throw new ResponseMessageException().WithException(DefaultCode.CREATE_FAILURE);
            var fileshort = new FileShortModel();
            fileshort.FileId = entity.Id;
            fileshort.FileName = entity.FileName;
            fileshort.Ext = entity.Ext;
            fileshort.Path = entity.Path;
            return fileshort;
        }

        public async Task<string> UploadFileCK(IFormFile file)
        {
            var name = string.Empty;
            try
            {
                if (file == null || (file != null && file.Length == 0))
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("File tải lên đang bị rỗng");
                }
                var fileName = Path.GetFileName(file.FileName);
                FileInfo fileInfo = new FileInfo(fileName);
                var extFile = fileInfo.Extension;
                if (fileName.Length > 100)
                {
                    throw new Exception("Tên tệp tin quá dài.");
                }
                var dateTime = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ssss");
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, "files/", dateTime);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!NKTM.Constants.DefaultFile.FILES.Contains(extFile))
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("File không thể cập nhật vào hệ thống");


                var newFileName = Guid.NewGuid().ToString() + extFile;
                var relativePath = Path.Combine("", dateTime, newFileName);
                var filePath = Path.Combine("files/", relativePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var entity = new FileModel();
                entity.FileName = fileName;
                entity.SaveName = newFileName;
                entity.Path = filePath;
                entity.Size = file.Length;
                entity.Ext = extFile;

                var result = await BaseMongoDb.CreateAsync(entity);

                if (result.Entity.Id == default || !result.Success)
                {
                    throw new ResponseMessageException()
                        .WithException(DefaultCode.CREATE_FAILURE);
                }
                var fileshort = new FileShortModel();
                fileshort.FileId = entity.Id;
                fileshort.FileName = entity.FileName;
                fileshort.Ext = entity.Ext;
                fileshort.Path = entity.Path;


                return $"https://localhost:5001/api/v1/files/view/{fileshort.FileId}";
            }
            catch (Exception e)
            {
                throw new ResponseMessageException().WithException(DefaultCode.CREATE_FAILURE);
            }
        }

    }
}
