using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class FileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadPath;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _uploadPath = Path.Combine(_environment.ContentRootPath, "Uploads");
        
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<FileUploadResult> UploadFileAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return new FileUploadResult { Success = false, Error = "No file provided" };

            if (file.Length > 10 * 1024 * 1024)
                return new FileUploadResult { Success = false, Error = "File size exceeds 10MB limit" };

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif", ".txt", ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                return new FileUploadResult { Success = false, Error = "File type not allowed" };

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new FileUploadResult 
            { 
                Success = true, 
                FileName = file.FileName,
                StoredFileName = fileName,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadPath = filePath
            };
        }
        catch (Exception ex)
        {
            return new FileUploadResult { Success = false, Error = $"Upload failed: {ex.Message}" };
        }
    }

    public async Task<List<FileUploadResult>> UploadFilesAsync(IEnumerable<IFormFile> files)
    {
        var results = new List<FileUploadResult>();
        
        foreach (var file in files)
        {
            var result = await UploadFileAsync(file);
            results.Add(result);
        }

        return results;
    }

    public bool DeleteFile(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_uploadPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerable<UploadedFileInfo> GetUploadedFiles()
    {
        var files = Directory.GetFiles(_uploadPath)
            .Select(filePath => new FileInfo(filePath))
            .Select(fileInfo => new UploadedFileInfo
            {
                FileName = Path.GetFileName(fileInfo.Name).Split('_', 2).LastOrDefault(),
                StoredFileName = fileInfo.Name,
                FileSize = fileInfo.Length,
                UploadDate = fileInfo.CreationTime,
                FilePath = fileInfo.FullName
            });

        return files;
    }
}