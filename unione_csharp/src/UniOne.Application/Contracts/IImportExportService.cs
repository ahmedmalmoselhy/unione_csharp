namespace UniOne.Application.Contracts;

public interface IImportExportService
{
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName);
    Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data);
    Task<ImportResult<T>> ImportFromExcelAsync<T>(Stream fileStream) where T : new();
}

public class ImportResult<T>
{
    public List<T> Data { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public bool Succeeded => !Errors.Any();
}
