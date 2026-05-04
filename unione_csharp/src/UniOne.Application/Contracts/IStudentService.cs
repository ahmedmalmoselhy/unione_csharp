using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllStudentsAsync(long? facultyId = null, long? departmentId = null, string? search = null);
    Task<StudentDto> GetStudentByIdAsync(long id);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto dto);
    Task<StudentDto> UpdateStudentAsync(long id, UpdateStudentDto dto);
    Task DeleteStudentAsync(long id);
    Task TransferStudentAsync(long id, TransferStudentDto dto);
    Task<byte[]> ExportStudentsAsync(long? facultyId = null, long? departmentId = null);
    Task<ImportResult<StudentImportRow>> ImportStudentsAsync(Stream fileStream);
}
