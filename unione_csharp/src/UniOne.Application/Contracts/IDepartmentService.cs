using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync(
        long? facultyId = null,
        string? search = null,
        string? type = null,
        bool? isActive = null,
        bool? noHead = null);
    Task<DepartmentDto> GetDepartmentByIdAsync(long id);
    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto, Stream? logoStream, string? logoFileName);
    Task<DepartmentDto> UpdateDepartmentAsync(long id, UpdateDepartmentDto dto, Stream? logoStream, string? logoFileName);
    Task DeleteDepartmentAsync(long id);
}
