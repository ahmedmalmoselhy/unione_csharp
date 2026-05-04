using Microsoft.AspNetCore.Hosting;
using UniOne.Application.Contracts;

namespace UniOne.Infrastructure.Persistence;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
    {
        var wwwrootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsPath = Path.Combine(wwwrootPath, "uploads", folderName);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        return Path.Combine("uploads", folderName, uniqueFileName).Replace("\\", "/");
    }

    public void DeleteFile(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;

        var wwwrootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var absolutePath = Path.Combine(wwwrootPath, filePath.TrimStart('/'));

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }
}
