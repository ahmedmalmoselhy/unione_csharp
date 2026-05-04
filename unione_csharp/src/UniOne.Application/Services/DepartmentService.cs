using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IAuditLogService _auditLog;
    private readonly ICurrentUserService _currentUserService;
    private readonly OrganizationMapper _mapper;

    public DepartmentService(
        IApplicationDbContext context,
        IFileService fileService,
        IAuditLogService auditLog,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileService = fileService;
        _auditLog = auditLog;
        _currentUserService = currentUserService;
        _mapper = new OrganizationMapper();
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync(
        long? facultyId = null,
        string? search = null,
        string? type = null,
        bool? isActive = null,
        bool? noHead = null)
    {
        var query = _context.Departments.AsQueryable();

        // Enforce scoping
        if (!_currentUserService.Roles.Contains("admin"))
        {
            var facultyIds = _currentUserService.FacultyScopeIds.ToList();
            var departmentIds = _currentUserService.DepartmentScopeIds.ToList();

            if (facultyIds.Any())
            {
                query = query.Where(d => d.FacultyId.HasValue && facultyIds.Contains(d.FacultyId.Value));
            }
            else if (departmentIds.Any())
            {
                query = query.Where(d => departmentIds.Contains(d.Id));
            }
            else
            {
                // If no admin role and no scopes, return empty unless they have other roles that allow viewing
                // For now, assume this is only for admin-like access
                return Enumerable.Empty<DepartmentDto>();
            }
        }

        if (facultyId.HasValue)
        {
            query = query.Where(d => d.FacultyId == facultyId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(d => d.Name.Contains(search) || d.NameAr.Contains(search) || d.Code.Contains(search));
        }

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<DepartmentType>(type, true, out var deptType))
        {
            query = query.Where(d => d.Type == deptType);
        }

        if (isActive.HasValue)
        {
            query = query.Where(d => d.IsActive == isActive.Value);
        }

        if (noHead.HasValue && noHead.Value)
        {
            query = query.Where(d => d.HeadId == null);
        }

        var departments = await query.OrderBy(d => d.Name).ToListAsync();
        return departments.Select(d => _mapper.ToDto(d));
    }

    public async Task<DepartmentDto> GetDepartmentByIdAsync(long id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) throw new KeyNotFoundException("Department not found");

        return _mapper.ToDto(department);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto, Stream? logoStream, string? logoFileName)
    {
        var department = _mapper.ToEntity(dto);
        department.Code = department.Code.ToUpper();
        department.Scope = department.FacultyId.HasValue ? DepartmentScope.Faculty : DepartmentScope.University;

        if (logoStream != null && logoFileName != null)
        {
            department.LogoPath = await _fileService.SaveFileAsync(logoStream, logoFileName, "departments");
        }

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "created",
            auditableType: "Department",
            auditableId: department.Id,
            description: $"Created department {department.Name}",
            newValues: new { department.Name, department.Code, department.Type, department.FacultyId, department.IsActive });

        return _mapper.ToDto(department);
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(long id, UpdateDepartmentDto dto, Stream? logoStream, string? logoFileName)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) throw new KeyNotFoundException("Department not found");

        var oldValues = new
        {
            department.Name,
            department.Code,
            department.FacultyId,
            department.IsActive
        };

        if (dto.RemoveLogo && department.LogoPath != null)
        {
            _fileService.DeleteFile(department.LogoPath);
            department.LogoPath = null;
        }

        if (logoStream != null && logoFileName != null)
        {
            if (department.LogoPath != null)
            {
                _fileService.DeleteFile(department.LogoPath);
            }
            department.LogoPath = await _fileService.SaveFileAsync(logoStream, logoFileName, "departments");
        }

        _mapper.UpdateDepartment(dto, department);
        department.Code = department.Code.ToUpper();
        department.Scope = department.FacultyId.HasValue ? DepartmentScope.Faculty : DepartmentScope.University;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "updated",
            auditableType: "Department",
            auditableId: department.Id,
            description: $"Updated department {department.Name}",
            oldValues: oldValues,
            newValues: new { dto.Name, Code = dto.Code.ToUpper(), dto.FacultyId, dto.IsActive });

        return _mapper.ToDto(department);
    }

    public async Task DeleteDepartmentAsync(long id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) throw new KeyNotFoundException("Department not found");

        if (department.IsMandatory)
        {
            throw new InvalidOperationException("This department is mandatory and cannot be deleted.");
        }

        var name = department.Name;

        try
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("This department cannot be deleted because it has associated records.");
        }

        await _auditLog.RecordAsync(
            action: "deleted",
            auditableType: "Department",
            auditableId: id,
            description: $"Deleted department {name}");
    }
}
