using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class ProfessorService : IProfessorService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLog;
    private readonly IImportExportService _importExport;
    private readonly PeopleMapper _mapper;

    public ProfessorService(
        IApplicationDbContext context,
        UserManager<User> userManager,
        ICurrentUserService currentUserService,
        IAuditLogService auditLog,
        IImportExportService importExport)
    {
        _context = context;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _auditLog = auditLog;
        _importExport = importExport;
        _mapper = new PeopleMapper();
    }

    public async Task<IEnumerable<ProfessorDto>> GetAllProfessorsAsync(long? departmentId = null, string? search = null)
    {
        var query = _context.Professors
            .Include(p => p.User)
            .Include(p => p.Department)
            .AsQueryable();

        query = ApplyScope(query);

        if (departmentId.HasValue) query = query.Where(p => p.DepartmentId == departmentId.Value);
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.User.FirstName.Contains(search) || 
                                    p.User.LastName.Contains(search) || 
                                    p.StaffNumber.Contains(search));
        }

        var professors = await query.OrderBy(p => p.User.FirstName).ToListAsync();
        return professors.Select(p => _mapper.ToDto(p));
    }

    public async Task<ProfessorDto> GetProfessorByIdAsync(long id)
    {
        var professor = await _context.Professors
            .Include(p => p.User)
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (professor == null) throw new KeyNotFoundException("Professor not found");
        EnsureCanAccess(professor);

        return _mapper.ToDto(professor);
    }

    public async Task<ProfessorDto> CreateProfessorAsync(CreateProfessorDto dto)
    {
        await EnsureCanAccessDepartmentAsync(dto.DepartmentId);

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

        await _userManager.AddToRoleAsync(user, "professor");

        var professor = _mapper.ToEntity(dto);
        professor.UserId = user.Id;

        _context.Professors.Add(professor);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "created",
            auditableType: "Professor",
            auditableId: professor.Id,
            description: $"Created professor {dto.FirstName} {dto.LastName}",
            newValues: new { professor.StaffNumber, professor.DepartmentId });

        return await GetProfessorByIdAsync(professor.Id);
    }

    public async Task<ProfessorDto> UpdateProfessorAsync(long id, UpdateProfessorDto dto)
    {
        var professor = await _context.Professors.Include(p => p.Department).FirstOrDefaultAsync(p => p.Id == id);
        if (professor == null) throw new KeyNotFoundException("Professor not found");
        EnsureCanAccess(professor);
        await EnsureCanAccessDepartmentAsync(dto.DepartmentId);

        var oldValues = new
        {
            professor.DepartmentId,
            professor.Specialization,
            professor.AcademicRank,
            professor.OfficeLocation,
            professor.HiredAt
        };

        _mapper.UpdateProfessor(dto, professor);
        professor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "updated",
            auditableType: "Professor",
            auditableId: professor.Id,
            description: "Updated professor profile info",
            oldValues: oldValues,
            newValues: dto);

        return await GetProfessorByIdAsync(id);
    }

    public async Task DeleteProfessorAsync(long id)
    {
        var professor = await _context.Professors
            .Include(p => p.User)
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (professor == null) throw new KeyNotFoundException("Professor not found");
        EnsureCanAccess(professor);

        var user = professor.User;
        _context.Professors.Remove(professor);
        await _userManager.DeleteAsync(user);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "deleted",
            auditableType: "Professor",
            auditableId: id,
            description: $"Deleted professor {user.FirstName} {user.LastName}");
    }

    public async Task<byte[]> ExportProfessorsAsync(long? departmentId = null)
    {
        var professors = await GetAllProfessorsAsync(departmentId);
        return await _importExport.ExportToExcelAsync(professors, "Professors");
    }

    public async Task<ImportResult<ProfessorImportRow>> ImportProfessorsAsync(Stream fileStream)
    {
        var result = await _importExport.ImportFromExcelAsync<ProfessorImportRow>(fileStream);
        if (!result.Succeeded) return result;

        foreach (var row in result.Data)
        {
            try
            {
                await CreateProfessorAsync(new CreateProfessorDto
                {
                    Email = row.Email,
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    NationalId = row.NationalId,
                    StaffNumber = row.StaffNumber,
                    DepartmentId = 1, // Placeholder
                    Specialization = row.Specialization,
                    AcademicRank = AcademicRank.Lecturer, // Placeholder, should parse row.AcademicRank
                    HiredAt = DateOnly.FromDateTime(DateTime.UtcNow)
                });
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Professor {row.Email}: {ex.Message}");
            }
        }

        return result;
    }

    private IQueryable<Professor> ApplyScope(IQueryable<Professor> query)
    {
        if (_currentUserService.Roles.Contains("admin"))
        {
            return query;
        }

        var facultyIds = _currentUserService.FacultyScopeIds.ToList();
        var departmentIds = _currentUserService.DepartmentScopeIds.ToList();

        if (facultyIds.Any())
        {
            return query.Where(p => p.Department.FacultyId.HasValue && facultyIds.Contains(p.Department.FacultyId.Value));
        }

        if (departmentIds.Any())
        {
            return query.Where(p => departmentIds.Contains(p.DepartmentId));
        }

        return query.Where(_ => false);
    }

    private void EnsureCanAccess(Professor professor)
    {
        if (_currentUserService.Roles.Contains("admin"))
        {
            return;
        }

        var facultyIds = _currentUserService.FacultyScopeIds.ToList();
        if (facultyIds.Any())
        {
            if (professor.Department.FacultyId.HasValue && facultyIds.Contains(professor.Department.FacultyId.Value))
            {
                return;
            }

            throw new UnauthorizedAccessException("Professor is outside the current faculty scope.");
        }

        var departmentIds = _currentUserService.DepartmentScopeIds.ToList();
        if (departmentIds.Any() && departmentIds.Contains(professor.DepartmentId))
        {
            return;
        }

        throw new UnauthorizedAccessException("Professor is outside the current department scope.");
    }

    private async Task EnsureCanAccessDepartmentAsync(long departmentId)
    {
        if (_currentUserService.Roles.Contains("admin"))
        {
            return;
        }

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId);
        if (department == null)
        {
            throw new InvalidOperationException("Department not found");
        }

        var facultyIds = _currentUserService.FacultyScopeIds.ToList();
        if (facultyIds.Any())
        {
            if (department.FacultyId.HasValue && facultyIds.Contains(department.FacultyId.Value))
            {
                return;
            }

            throw new UnauthorizedAccessException("Department is outside the current faculty scope.");
        }

        var departmentIds = _currentUserService.DepartmentScopeIds.ToList();
        if (departmentIds.Any() && departmentIds.Contains(departmentId))
        {
            return;
        }

        throw new UnauthorizedAccessException("Department is outside the current department scope.");
    }
}
