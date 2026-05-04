using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(long? departmentId = null, string? search = null);
    Task<EmployeeDto> GetEmployeeByIdAsync(long id);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<EmployeeDto> UpdateEmployeeAsync(long id, UpdateEmployeeDto dto);
    Task DeleteEmployeeAsync(long id);
    Task<byte[]> ExportEmployeesAsync(long? departmentId = null);
}
