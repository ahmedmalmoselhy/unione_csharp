using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class FacultyService : IFacultyService
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IAuditLogService _auditLog;
    private readonly OrganizationMapper _mapper;

    public FacultyService(
        IApplicationDbContext context,
        IFileService fileService,
        IAuditLogService auditLog)
    {
        _context = context;
        _fileService = fileService;
        _auditLog = auditLog;
        _mapper = new OrganizationMapper();
    }

    public async Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync(string? search = null, bool? isActive = null, string? enrollmentType = null)
    {
        var query = _context.Faculties.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(f => f.Name.Contains(search) || f.NameAr.Contains(search) || f.Code.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(f => f.IsActive == isActive.Value);
        }

        if (!string.IsNullOrEmpty(enrollmentType) && Enum.TryParse<EnrollmentType>(enrollmentType, true, out var type))
        {
            query = query.Where(f => f.EnrollmentType == type);
        }

        var faculties = await query.OrderBy(f => f.Name).ToListAsync();
        return faculties.Select(f => _mapper.ToDto(f));
    }

    public async Task<FacultyDto> GetFacultyByIdAsync(long id)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null) throw new KeyNotFoundException("Faculty not found");

        return _mapper.ToDto(faculty);
    }

    public async Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto dto, Stream? logoStream, string? logoFileName)
    {
        var faculty = _mapper.ToEntity(dto);
        faculty.Code = faculty.Code.ToUpper();

        if (logoStream != null && logoFileName != null)
        {
            faculty.LogoPath = await _fileService.SaveFileAsync(logoStream, logoFileName, "faculties");
        }

        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "created",
            auditableType: "Faculty",
            auditableId: faculty.Id,
            description: $"Created faculty {faculty.Name}",
            newValues: new { faculty.Name, faculty.Code, faculty.EnrollmentType });

        return _mapper.ToDto(faculty);
    }

    public async Task<FacultyDto> UpdateFacultyAsync(long id, UpdateFacultyDto dto, Stream? logoStream, string? logoFileName)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null) throw new KeyNotFoundException("Faculty not found");

        var oldValues = new
        {
            faculty.Name,
            faculty.NameAr,
            faculty.Code,
            faculty.EnrollmentType,
            faculty.IsActive
        };

        if (dto.RemoveLogo && faculty.LogoPath != null)
        {
            _fileService.DeleteFile(faculty.LogoPath);
            faculty.LogoPath = null;
        }

        if (logoStream != null && logoFileName != null)
        {
            if (faculty.LogoPath != null)
            {
                _fileService.DeleteFile(faculty.LogoPath);
            }
            faculty.LogoPath = await _fileService.SaveFileAsync(logoStream, logoFileName, "faculties");
        }

        _mapper.UpdateFaculty(dto, faculty);
        faculty.Code = faculty.Code.ToUpper();
        faculty.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "updated",
            auditableType: "Faculty",
            auditableId: faculty.Id,
            description: $"Updated faculty {faculty.Name}",
            oldValues: oldValues,
            newValues: new { dto.Name, Code = dto.Code.ToUpper(), dto.EnrollmentType, dto.IsActive });

        return _mapper.ToDto(faculty);
    }

    public async Task DeleteFacultyAsync(long id)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null) throw new KeyNotFoundException("Faculty not found");

        var name = faculty.Name;

        try
        {
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("This faculty cannot be deleted because it has associated records.");
        }

        await _auditLog.RecordAsync(
            action: "deleted",
            auditableType: "Faculty",
            auditableId: id,
            description: $"Deleted faculty {name}");
    }
}
