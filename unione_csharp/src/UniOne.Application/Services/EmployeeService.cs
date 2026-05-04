using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IAuditLogService _auditLog;
    private readonly IImportExportService _importExport;
    private readonly PeopleMapper _mapper;

    public EmployeeService(
        IApplicationDbContext context,
        UserManager<User> userManager,
        IAuditLogService auditLog,
        IImportExportService importExport)
    {
        _context = context;
        _userManager = userManager;
        _auditLog = auditLog;
        _importExport = importExport;
        _mapper = new PeopleMapper();
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(long? departmentId = null, string? search = null)
    {
        var query = _context.Employees
            .Include(e => e.User)
            .Include(e => e.Department)
            .AsQueryable();

        if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId.Value);
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.User.FirstName.Contains(search) || 
                                    e.User.LastName.Contains(search) || 
                                    e.StaffNumber.Contains(search));
        }

        var employees = await query.OrderBy(e => e.User.FirstName).ToListAsync();
        return employees.Select(e => _mapper.ToDto(e));
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(long id)
    {
        var employee = await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) throw new KeyNotFoundException("Employee not found");

        return _mapper.ToDto(employee);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            NationalId = dto.NationalId,
            Phone = dto.Phone,
            IsActive = true,
            MustChangePassword = true
        };

        var result = await _userManager.CreateAsync(user, "Welcome@123");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "employee");

        var employee = _mapper.ToEntity(dto);
        employee.UserId = user.Id;

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "created",
            auditableType: "Employee",
            auditableId: employee.Id,
            description: $"Created employee {dto.FirstName} {dto.LastName}",
            newValues: new { employee.StaffNumber, employee.DepartmentId });

        return await GetEmployeeByIdAsync(employee.Id);
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(long id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) throw new KeyNotFoundException("Employee not found");

        var oldValues = new
        {
            employee.DepartmentId,
            employee.JobTitle,
            employee.EmploymentType,
            employee.Salary,
            employee.HiredAt,
            employee.TerminatedAt
        };

        _mapper.UpdateEmployee(dto, employee);
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "updated",
            auditableType: "Employee",
            auditableId: employee.Id,
            description: "Updated employee profile info",
            oldValues: oldValues,
            newValues: dto);

        return await GetEmployeeByIdAsync(id);
    }

    public async Task DeleteEmployeeAsync(long id)
    {
        var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) throw new KeyNotFoundException("Employee not found");

        var user = employee.User;
        _context.Employees.Remove(employee);
        await _userManager.DeleteAsync(user);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "deleted",
            auditableType: "Employee",
            auditableId: id,
            description: $"Deleted employee {user.FirstName} {user.LastName}");
    }

    public async Task<byte[]> ExportEmployeesAsync(long? departmentId = null)
    {
        var employees = await GetAllEmployeesAsync(departmentId);
        return await _importExport.ExportToExcelAsync(employees, "Employees");
    }
}
