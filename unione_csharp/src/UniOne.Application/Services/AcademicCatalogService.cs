using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class AcademicCatalogService : IAcademicCatalogService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLog;
    private readonly CatalogMapper _mapper;

    public AcademicCatalogService(IApplicationDbContext context, IAuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
        _mapper = new CatalogMapper();
    }

    // Academic Terms
    public async Task<IEnumerable<AcademicTermDto>> GetAllTermsAsync()
    {
        var terms = await _context.AcademicTerms.OrderByDescending(t => t.AcademicYear).ThenBy(t => t.Semester).ToListAsync();
        return terms.Select(t => _mapper.ToDto(t));
    }

    public async Task<AcademicTermDto> GetTermByIdAsync(long id)
    {
        var term = await _context.AcademicTerms.FindAsync(id);
        if (term == null) throw new KeyNotFoundException("Term not found");
        return _mapper.ToDto(term);
    }

    public async Task<AcademicTermDto> CreateTermAsync(CreateAcademicTermDto dto)
    {
        var term = _mapper.ToEntity(dto);
        _context.AcademicTerms.Add(term);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("created", "AcademicTerm", term.Id, $"Created term {term.Name}");
        return _mapper.ToDto(term);
    }

    public async Task<AcademicTermDto> UpdateTermAsync(long id, UpdateAcademicTermDto dto)
    {
        var term = await _context.AcademicTerms.FindAsync(id);
        if (term == null) throw new KeyNotFoundException("Term not found");

        _mapper.UpdateTerm(dto, term);
        term.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("updated", "AcademicTerm", id, $"Updated term {term.Name}");
        return _mapper.ToDto(term);
    }

    public async Task DeleteTermAsync(long id)
    {
        var term = await _context.AcademicTerms.FindAsync(id);
        if (term == null) throw new KeyNotFoundException("Term not found");

        _context.AcademicTerms.Remove(term);
        await _context.SaveChangesAsync();
        await _auditLog.RecordAsync("deleted", "AcademicTerm", id, $"Deleted term {term.Name}");
    }

    // Courses
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(string? search = null)
    {
        var query = _context.Courses.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
        }
        var courses = await query.OrderBy(c => c.Code).ToListAsync();
        return courses.Select(c => _mapper.ToDto(c));
    }

    public async Task<CourseDto> GetCourseByIdAsync(long id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found");
        return _mapper.ToDto(course);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = _mapper.ToEntity(dto);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("created", "Course", course.Id, $"Created course {course.Code}");
        return _mapper.ToDto(course);
    }

    public async Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found");

        _mapper.UpdateCourse(dto, course);
        course.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("updated", "Course", id, $"Updated course {course.Code}");
        return _mapper.ToDto(course);
    }

    public async Task DeleteCourseAsync(long id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found");

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        await _auditLog.RecordAsync("deleted", "Course", id, $"Deleted course {course.Code}");
    }

    // Sections
    public async Task<IEnumerable<SectionDto>> GetAllSectionsAsync(long? courseId = null, long? termId = null, long? professorId = null)
    {
        var query = _context.Sections
            .Include(s => s.Course)
            .Include(s => s.Professor).ThenInclude(p => p.User)
            .Include(s => s.AcademicTerm)
            .AsQueryable();

        if (courseId.HasValue) query = query.Where(s => s.CourseId == courseId.Value);
        if (termId.HasValue) query = query.Where(s => s.AcademicTermId == termId.Value);
        if (professorId.HasValue) query = query.Where(s => s.ProfessorId == professorId.Value);

        var sections = await query.ToListAsync();
        return sections.Select(s => _mapper.ToDto(s));
    }

    public async Task<SectionDto> GetSectionByIdAsync(long id)
    {
        var section = await _context.Sections
            .Include(s => s.Course)
            .Include(s => s.Professor).ThenInclude(p => p.User)
            .Include(s => s.AcademicTerm)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (section == null) throw new KeyNotFoundException("Section not found");
        return _mapper.ToDto(section);
    }

    public async Task<SectionDto> CreateSectionAsync(CreateSectionDto dto)
    {
        var section = _mapper.ToEntity(dto);
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("created", "Section", section.Id, $"Created section for course {dto.CourseId} in term {dto.AcademicTermId}");
        return await GetSectionByIdAsync(section.Id);
    }

    public async Task<SectionDto> UpdateSectionAsync(long id, UpdateSectionDto dto)
    {
        var section = await _context.Sections.FindAsync(id);
        if (section == null) throw new KeyNotFoundException("Section not found");

        _mapper.UpdateSection(dto, section);
        section.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("updated", "Section", id, $"Updated section {id}");
        return await GetSectionByIdAsync(id);
    }

    public async Task DeleteSectionAsync(long id)
    {
        var section = await _context.Sections.FindAsync(id);
        if (section == null) throw new KeyNotFoundException("Section not found");

        _context.Sections.Remove(section);
        await _context.SaveChangesAsync();
        await _auditLog.RecordAsync("deleted", "Section", id, $"Deleted section {id}");
    }
}
