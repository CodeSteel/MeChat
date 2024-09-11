﻿using MeChat.Contexts;
using MeChat.Models;

namespace MeChat.Services;

public class FileUploader
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDataContext _dataContext;
    
    public FileUploader(IWebHostEnvironment environment, ApplicationDataContext dataContext)
    {
        _environment = environment;
        _dataContext = dataContext;
    }

    public async Task<FileUpload> UploadFile(User user, IFormFile file)
    {
        string fullPath = GetFilePath(user, file, out string databaseFilePath);
        
        if (file.Length > 0)
        {
            await using FileStream stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
        
        FileUpload upload = CreateFileUpload();
        upload.Path = databaseFilePath;
        
        _dataContext.FileUploads.Add(upload);
        await _dataContext.SaveChangesAsync();
        
        return upload;
    }
    
    public bool GetFileExists(string filePath, out FileUpload? fileUpload)
    {
        fileUpload = _dataContext.FileUploads.FirstOrDefault(x => x.Path == filePath);
        return fileUpload != null;
    }
    
    public string GetFilePath(User user, IFormFile file, out string databaseFilePath)
    {
        string subPath = $"uploads\\{user.Id}";
        string path = Path.Combine(_environment.WebRootPath, subPath);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        string fileName = Path.GetFileName(file.FileName);
        databaseFilePath = "/" + Path.Combine(subPath, fileName).Replace("\\", "/").Replace(" ", "%20");

        return Path.Combine(path, fileName);
    }
    
    private FileUpload CreateFileUpload()
    {
        try
        {
            return Activator.CreateInstance<FileUpload>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. ");
        }
    }

}