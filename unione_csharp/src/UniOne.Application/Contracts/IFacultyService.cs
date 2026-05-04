using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IFacultyService
{
    Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync(string? search = null, bool? isActive = null, string? enrollmentType = null);
    Task<FacultyDto> GetFacultyByIdAsync(long id);
    Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto dto, Stream? logoStream, string? logoFileName);
    Task<FacultyDto> UpdateFacultyAsync(long id, UpdateFacultyDto dto, Stream? logoStream, string? logoFileName);
    Task DeleteFacultyAsync(long id);
}
