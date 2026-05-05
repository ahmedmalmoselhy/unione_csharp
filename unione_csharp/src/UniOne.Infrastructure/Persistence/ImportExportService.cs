using System.Globalization;
using ClosedXML.Excel;
using CsvHelper;
using UniOne.Application.Contracts;

namespace UniOne.Infrastructure.Persistence;

public class ImportExportService : IImportExportService
{
    public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);
        worksheet.Cell(1, 1).InsertTable(data);
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(data);
        await writer.FlushAsync();
        return stream.ToArray();
    }

    public async Task<ImportResult<T>> ImportFromExcelAsync<T>(Stream fileStream) where T : new()
    {
        var result = new ImportResult<T>();
        try
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                result.Errors.Add("Excel file contains no worksheets.");
                return result;
            }
            var range = worksheet.RangeUsed();
            if (range == null)
            {
                result.Errors.Add("Excel worksheet is empty.");
                return result;
            }
            var rows = range.RowsUsed().Skip(1); // Skip header

            var properties = typeof(T).GetProperties();
            var headers = range.Row(1).Cells().Select(c => c.Value.ToString().Trim()).ToList();

            foreach (var row in rows)
            {
                var item = new T();
                bool hasError = false;
                for (int i = 0; i < headers.Count; i++)
                {
                    var prop = properties.FirstOrDefault(p => p.Name.Equals(headers[i], StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        try
                        {
                            var cellValue = row.Cell(i + 1).Value;
                            var convertedValue = Convert.ChangeType(cellValue.ToString(), prop.PropertyType);
                            prop.SetValue(item, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Row {row.RowNumber()}: Invalid value for {headers[i]}. {ex.Message}");
                            hasError = true;
                        }
                    }
                }
                if (!hasError)
                {
                    result.Data.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Failed to process Excel file: {ex.Message}");
        }

        return result;
    }
}
