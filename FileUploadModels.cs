using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class FileUploadResult
{
    public bool Success { get; set; }
    public string FileName { get; set; }
    public string StoredFileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public string UploadPath { get; set; }
    public string Error { get; set; }
}

public class UploadedFileInfo
{
    public string FileName { get; set; }
    public string StoredFileName { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
    public string FilePath { get; set; }
    public string FileSizeFormatted => FormatFileSize(FileSize);

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double len = bytes;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

public class UploadingFile
{
    public string FileName { get; set; }
    public long Size { get; set; }
    public int Progress { get; set; }
    public bool Error { get; set; }
}

public class HttpFile : IFormFile
{
    private readonly byte[] _data;

    public HttpFile(string fileName, string name, byte[] data, string contentType)
    {
        FileName = fileName;
        Name = name;
        _data = data;
        ContentType = contentType;
        Length = data.Length;
    }

    public string ContentType { get; }
    public string ContentDisposition => null;
    public IHeaderDictionary Headers => new HeaderDictionary();
    public long Length { get; }
    public string Name { get; }
    public string FileName { get; }

    public void CopyTo(Stream target)
    {
        target.Write(_data, 0, _data.Length);
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        await target.WriteAsync(_data, cancellationToken);
    }

    public Stream OpenReadStream()
    {
        return new MemoryStream(_data);
    }
}