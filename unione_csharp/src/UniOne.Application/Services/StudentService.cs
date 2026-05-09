using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class StudentService : IStudentService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLog;
    private readonly IImportExportService _importExport;
    private readonly PeopleMapper _mapper;

    public StudentService(
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

    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync(long? facultyId = null, long? departmentId = null, string? search = null)
    {
        var query = _context.Students
            .Include(s => s.User)
            .Include(s => s.Faculty)
            .Include(s => s.Department)
            .AsQueryable();

        // Enforce scoping
        if (!_currentUserService.Roles.Contains("admin"))
        {
            var facultyIds = _currentUserService.FacultyScopeIds.ToList();
            var departmentIds = _currentUserService.DepartmentScopeIds.ToList();

            if (facultyIds.Any())
            {
                query = query.Where(s => facultyIds.Contains(s.FacultyId));
            }
            else if (departmentIds.Any())
            {
                query = query.Where(s => s.DepartmentId.HasValue && departmentIds.Contains(s.DepartmentId.Value));
            }
            else
            {
                return Enumerable.Empty<StudentDto>();
            }
        }

        if (facultyId.HasValue) query = query.Where(s => s.FacultyId == facultyId.Value);
        if (departmentId.HasValue) query = query.Where(s => s.DepartmentId == departmentId.Value);
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.User.FirstName.Contains(search) || 
                                    s.User.LastName.Contains(search) || 
                                    s.StudentNumber.Contains(search));
        }

        var students = await query.OrderBy(s => s.User.FirstName).ToListAsync();
        return students.Select(s => _mapper.ToDto(s));
    }

    public async Task<StudentDto> GetStudentByIdAsync(long id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.Faculty)
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) throw new KeyNotFoundException("Student not found");

        return _mapper.ToDto(student);
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto)
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

        var result = await _userManager.CreateAsync(user, "Welcome@123"); // Default password
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "student");

        var student = _mapper.ToEntity(dto);
        student.UserId = user.Id;

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "created",
            auditableType: "Student",
            auditableId: student.Id,
            description: $"Created student {dto.FirstName} {dto.LastName}",
            newValues: new { student.StudentNumber, student.FacultyId, student.DepartmentId });

        return await GetStudentByIdAsync(student.Id);
    }

    public async Task<StudentDto> UpdateStudentAsync(long id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) throw new KeyNotFoundException("Student not found");

        var oldValues = new
        {
            student.DepartmentId,
            student.AcademicYear,
            student.Semester,
            student.EnrollmentStatus,
            student.AcademicStanding,
            student.Gpa,
            student.GraduatedAt
        };

        _mapper.UpdateStudent(dto, student);
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "updated",
            auditableType: "Student",
            auditableId: student.Id,
            description: "Updated student academic info",
            oldValues: oldValues,
            newValues: dto);

        return await GetStudentByIdAsync(id);
    }

    public async Task DeleteStudentAsync(long id)
    {
        var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) throw new KeyNotFoundException("Student not found");

        var user = student.User;
        _context.Students.Remove(student);
        await _userManager.DeleteAsync(user);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "deleted",
            auditableType: "Student",
            auditableId: id,
            description: $"Deleted student {user.FirstName} {user.LastName}");
    }

    public async Task TransferStudentAsync(long id, TransferStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) throw new KeyNotFoundException("Student not found");

        var fromDeptId = student.DepartmentId;
        if (fromDeptId == dto.ToDepartmentId) return;

        var history = new StudentDepartmentHistory
        {
            StudentId = student.Id,
            FromDepartmentId = fromDeptId,
            ToDepartmentId = dto.ToDepartmentId,
            SwitchedAt = DateTime.UtcNow,
            SwitchedBy = _currentUserService.UserId ?? 0,
            Note = dto.Note
        };

        student.DepartmentId = dto.ToDepartmentId;
        student.UpdatedAt = DateTime.UtcNow;

        _context.StudentDepartmentHistories.Add(history);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "transferred",
            auditableType: "Student",
            auditableId: student.Id,
            description: $"Transferred student to department {dto.ToDepartmentId}",
            oldValues: new { FromDepartmentId = fromDeptId },
            newValues: new { ToDepartmentId = dto.ToDepartmentId, dto.Note });
    }

    public async Task<byte[]> ExportStudentsAsync(long? facultyId = null, long? departmentId = null)
    {
        var students = await GetAllStudentsAsync(facultyId, departmentId);
        return await _importExport.ExportToExcelAsync(students, "Students");
    }

    public async Task<ImportResult<StudentImportRow>> ImportStudentsAsync(Stream fileStream)
    {
        var result = await _importExport.ImportFromExcelAsync<StudentImportRow>(fileStream);
        if (!result.Succeeded) return result;

        foreach (var row in result.Data)
        {
            try
            {
                // Simple implementation, in real app we'd resolve Faculty/Dept by name
                // For now, let's assume they provide IDs or we have a mapping
                // This is a placeholder for the logic in Laravel's StudentsImport
                await CreateStudentAsync(new CreateStudentDto
                {
                    Email = row.Email,
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    NationalId = row.NationalId,
                    StudentNumber = row.StudentNumber,
                    FacultyId = 1, // Placeholder
                    EnrolledAt = DateOnly.FromDateTime(DateTime.UtcNow)
                });
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Student {row.Email}: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<TranscriptDto> GetTranscriptAsync(long studentId)
    {
        var student = await GetStudentByIdAsync(studentId);
        var grades = await _context.Grades
            .Include(g => g.Section)
            .ThenInclude(s => s.Course)
            .Include(g => g.Section)
            .ThenInclude(s => s.AcademicTerm)
            .Where(g => g.StudentId == studentId && g.IsPublished)
            .ToListAsync();

        var termGpas = await _context.StudentTermGpas
            .Where(tg => tg.StudentId == studentId)
            .ToDictionaryAsync(tg => tg.AcademicTermId);

        var transcript = new TranscriptDto
        {
            Student = student,
            CumulativeGpa = student.Gpa ?? 0,
            TotalCreditsEarned = grades.Where(g => g.GradeLetter != "F").Sum(g => g.Section.Course.CreditHours),
            Terms = grades.GroupBy(g => g.Section.AcademicTermId)
                .Select(g =>
                {
                    var term = g.First().Section.AcademicTerm;
                    var termGpa = termGpas.GetValueOrDefault(term.Id);
                    return new TranscriptTermDto
                    {
                        AcademicTermId = term.Id,
                        TermName = term.Name,
                        TermGpa = termGpa?.TermGpa ?? 0,
                        CreditsAttempted = termGpa?.CreditsAttempted ?? 0,
                        CreditsEarned = termGpa?.CreditsEarned ?? 0,
                        Courses = g.Select(c => new TranscriptCourseDto
                        {
                            CourseCode = c.Section.Course.Code,
                            CourseName = c.Section.Course.Name,
                            CreditHours = c.Section.Course.CreditHours,
                            GradeLetter = c.GradeLetter,
                            GradePoints = c.GradePoints
                        }).ToList()
                    };
                }).OrderBy(t => t.AcademicTermId).ToList()
        };

        return transcript;
    }

    public async Task<ScheduleDto> GetScheduleAsync(long studentId, long? academicTermId = null)
    {
        var query = _context.Enrollments
            .Include(e => e.Section)
            .ThenInclude(s => s.Course)
            .Include(e => e.Section)
            .ThenInclude(s => s.Professor)
            .ThenInclude(p => p.User)
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentRecordStatus.Registered);

        if (academicTermId.HasValue)
        {
            query = query.Where(e => e.AcademicTermId == academicTermId.Value);
        }
        else
        {
            // If no term specified, get active term(s)
            var activeTermIds = await _context.AcademicTerms
                .Where(t => t.IsActive)
                .Select(t => t.Id)
                .ToListAsync();
            query = query.Where(e => e.AcademicTermId != 0 && activeTermIds.Contains(e.AcademicTermId));
        }

        var enrollments = await query.ToListAsync();

        return new ScheduleDto
        {
            Items = enrollments.Select(e => new ScheduleItemDto
            {
                SectionId = e.SectionId,
                CourseCode = e.Section.Course.Code,
                CourseName = e.Section.Course.Name,
                SectionNumber = e.Section.SectionNumber,
                ProfessorName = $"{e.Section.Professor.User.FirstName} {e.Section.Professor.User.LastName}",
                Schedule = e.Section.Schedule
            }).ToList()
        };
    }
}
