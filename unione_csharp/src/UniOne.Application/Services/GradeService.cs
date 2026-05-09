using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class GradeService : IGradeService
{
    private readonly IApplicationDbContext _context;
    private readonly IGpaService _gpaService;

    public GradeService(IApplicationDbContext context, IGpaService gpaService)
    {
        _context = context;
        _gpaService = gpaService;
    }

    public async Task<IEnumerable<GradeDto>> GetSectionGrades(long sectionId)
    {
        return await _context.Grades
            .Where(g => g.SectionId == sectionId)
            .Select(g => new GradeDto
            {
                Id = g.Id,
                EnrollmentId = g.EnrollmentId,
                StudentId = g.StudentId,
                SectionId = g.SectionId,
                Score = g.Score,
                GradeLetter = g.GradeLetter,
                GradePoints = g.GradePoints,
                IsPublished = g.IsPublished,
                PublishedAt = g.PublishedAt
            })
            .ToListAsync();
    }

    public async Task<GradeDto> SubmitGrade(long sectionId, SubmitGradeDto dto)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.Id == dto.EnrollmentId && e.SectionId == sectionId);

        if (enrollment == null)
        {
            throw new Exception("Enrollment not found in this section.");
        }

        var grade = await _context.Grades
            .FirstOrDefaultAsync(g => g.EnrollmentId == dto.EnrollmentId);

        if (grade == null)
        {
            grade = new Grade
            {
                EnrollmentId = dto.EnrollmentId,
                StudentId = enrollment.StudentId,
                SectionId = sectionId
            };
            _context.Grades.Add(grade);
        }

        grade.Score = dto.Score;
        grade.GradeLetter = _gpaService.GetGradeLetter(dto.Score);
        grade.GradePoints = _gpaService.CalculateGradePoints(grade.GradeLetter);
        grade.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(default);

        return new GradeDto
        {
            Id = grade.Id,
            EnrollmentId = grade.EnrollmentId,
            StudentId = grade.StudentId,
            SectionId = grade.SectionId,
            Score = grade.Score,
            GradeLetter = grade.GradeLetter,
            GradePoints = grade.GradePoints,
            IsPublished = grade.IsPublished,
            PublishedAt = grade.PublishedAt
        };
    }

    public async Task PublishGrades(long sectionId)
    {
        var grades = await _context.Grades
            .Where(g => g.SectionId == sectionId && !g.IsPublished)
            .ToListAsync();

        foreach (var grade in grades)
        {
            grade.IsPublished = true;
            grade.PublishedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(default);

        // Recalculate GPA for all affected students
        var studentIds = grades.Select(g => g.StudentId).Distinct();
        foreach (var studentId in studentIds)
        {
            await _gpaService.RecalculateStudentGpa(studentId);
        }
    }

    public async Task<IEnumerable<GradeDto>> GetStudentGrades(long studentId, long? academicTermId = null)
    {
        var query = _context.Grades
            .Include(g => g.Section)
            .Where(g => g.StudentId == studentId && g.IsPublished);

        if (academicTermId.HasValue)
        {
            query = query.Where(g => g.Section.AcademicTermId == academicTermId.Value);
        }

        return await query
            .Select(g => new GradeDto
            {
                Id = g.Id,
                EnrollmentId = g.EnrollmentId,
                StudentId = g.StudentId,
                SectionId = g.SectionId,
                Score = g.Score,
                GradeLetter = g.GradeLetter,
                GradePoints = g.GradePoints,
                IsPublished = g.IsPublished,
                PublishedAt = g.PublishedAt
            })
            .ToListAsync();
    }
}
