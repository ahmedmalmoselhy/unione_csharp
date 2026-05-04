namespace UniOne.Application.Contracts;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
    void DeleteFile(string? filePath);
}
