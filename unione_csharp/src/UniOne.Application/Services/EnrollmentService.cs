using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLog;
    private readonly ICurrentUserService _currentUserService;

    public EnrollmentService(
        IApplicationDbContext context,
        IAuditLogService auditLog,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _auditLog = auditLog;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<EnrollmentDto>> GetStudentEnrollmentsAsync(long studentId)
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Section).ThenInclude(s => s.Course)
            .Include(e => e.AcademicTerm)
            .Where(e => e.StudentId == studentId && e.Status != EnrollmentRecordStatus.Dropped)
            .ToListAsync();

        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            SectionId = e.SectionId,
            CourseName = e.Section.Course.Name,
            CourseCode = e.Section.Course.Code,
            AcademicTermId = e.AcademicTermId,
            AcademicTermName = e.AcademicTerm.Name,
            Status = e.Status,
            RegisteredAt = e.RegisteredAt
        });
    }

    public async Task<EnrollmentDto> EnrollStudentAsync(CreateEnrollmentDto dto)
    {
        var section = await _context.Sections
            .Include(s => s.AcademicTerm)
            .Include(s => s.Course).ThenInclude(c => c.Prerequisites)
            .FirstOrDefaultAsync(s => s.Id == dto.SectionId);

        if (section == null) throw new KeyNotFoundException("Section not found");
        if (!section.IsActive) throw new InvalidOperationException("Section is not active.");

        var term = section.AcademicTerm;
        var now = DateTime.UtcNow;

        if (term.RegistrationStartsAt.HasValue && now < term.RegistrationStartsAt.Value)
            throw new InvalidOperationException("Registration period has not started yet.");
        
        if (term.RegistrationEndsAt.HasValue && now > term.RegistrationEndsAt.Value)
            throw new InvalidOperationException("Registration period has ended.");

        var alreadyEnrolled = await _context.Enrollments
            .AnyAsync(e => e.StudentId == dto.StudentId && e.SectionId == dto.SectionId && 
                          (e.Status == EnrollmentRecordStatus.Registered || e.Status == EnrollmentRecordStatus.Completed));

        if (alreadyEnrolled) throw new InvalidOperationException("Already enrolled in this section.");

        // Prerequisite check
        var prerequisiteIds = section.Course.Prerequisites.Select(p => p.PrerequisiteId).ToList();
        if (prerequisiteIds.Any())
        {
            var completedCourseIds = await _context.Enrollments
                .Include(e => e.Section)
                .Where(e => e.StudentId == dto.StudentId && e.Status == EnrollmentRecordStatus.Completed)
                .Select(e => e.Section.CourseId)
                .Distinct()
                .ToListAsync();

            var missing = prerequisiteIds.Except(completedCourseIds).ToList();
            if (missing.Any())
            {
                throw new InvalidOperationException("Prerequisites not satisfied.");
            }
        }

        // Capacity check
        var filledSpots = await _context.Enrollments
            .CountAsync(e => e.SectionId == dto.SectionId && 
                           (e.Status == EnrollmentRecordStatus.Registered || e.Status == EnrollmentRecordStatus.Completed));

        if (filledSpots >= section.Capacity)
        {
            // Join waitlist
            var alreadyWaiting = await _context.EnrollmentWaitlists
                .AnyAsync(w => w.StudentId == dto.StudentId && w.SectionId == dto.SectionId);

            if (alreadyWaiting) throw new InvalidOperationException("Already on the waitlist.");

            var maxPosition = await _context.EnrollmentWaitlists
                .Where(w => w.SectionId == dto.SectionId)
                .MaxAsync(w => (ushort?)w.Position) ?? 0;

            var waitlist = new EnrollmentWaitlist
            {
                StudentId = dto.StudentId,
                SectionId = dto.SectionId,
                AcademicTermId = term.Id,
                Position = (ushort)(maxPosition + 1),
                JoinedAt = now
            };

            _context.EnrollmentWaitlists.Add(waitlist);
            await _context.SaveChangesAsync();

            throw new InvalidOperationException($"Section is full. You have been added to the waitlist at position {waitlist.Position}.");
        }

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            SectionId = dto.SectionId,
            AcademicTermId = term.Id,
            Status = EnrollmentRecordStatus.Registered,
            RegisteredAt = now
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync("created", "Enrollment", enrollment.Id, $"Student {dto.StudentId} enrolled in section {dto.SectionId}");

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            SectionId = enrollment.SectionId,
            CourseName = section.Course.Name,
            CourseCode = section.Course.Code,
            AcademicTermId = enrollment.AcademicTermId,
            AcademicTermName = term.Name,
            Status = enrollment.Status,
            RegisteredAt = enrollment.RegisteredAt
        };
    }

    public async Task DropEnrollmentAsync(long enrollmentId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Section).ThenInclude(s => s.AcademicTerm)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null) throw new KeyNotFoundException("Enrollment not found");
        if (enrollment.Status == EnrollmentRecordStatus.Dropped) throw new InvalidOperationException("Already dropped.");

        var term = enrollment.Section.AcademicTerm;
        var now = DateTime.UtcNow;

        if (term.RegistrationStartsAt.HasValue && now < term.RegistrationStartsAt.Value)
            throw new InvalidOperationException("Drop period has not started yet.");

        if (term.RegistrationEndsAt.HasValue && now > term.RegistrationEndsAt.Value)
            throw new InvalidOperationException("Drop period has ended.");

        enrollment.Status = EnrollmentRecordStatus.Dropped;
        enrollment.DroppedAt = now;
        enrollment.UpdatedAt = now;

        // Auto-promote from waitlist
        var next = await _context.EnrollmentWaitlists
            .Where(w => w.SectionId == enrollment.SectionId)
            .OrderBy(w => w.Position)
            .FirstOrDefaultAsync();

        if (next != null)
        {
            var promotedEnrollment = new Enrollment
            {
                StudentId = next.StudentId,
                SectionId = next.SectionId,
                AcademicTermId = next.AcademicTermId,
                Status = EnrollmentRecordStatus.Registered,
                RegisteredAt = now
            };

            _context.Enrollments.Add(promotedEnrollment);
            _context.EnrollmentWaitlists.Remove(next);

            // Re-order waitlist
            var remaining = await _context.EnrollmentWaitlists
                .Where(w => w.SectionId == enrollment.SectionId && w.Id != next.Id)
                .OrderBy(w => w.Position)
                .ToListAsync();

            for (int i = 0; i < remaining.Count; i++)
            {
                remaining[i].Position = (ushort)(i + 1);
            }
        }

        await _context.SaveChangesAsync();
        await _auditLog.RecordAsync("dropped", "Enrollment", enrollmentId, $"Student {enrollment.StudentId} dropped section {enrollment.SectionId}");
    }

    public async Task<IEnumerable<EnrollmentDto>> GetSectionEnrollmentsAsync(long sectionId)
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Section).ThenInclude(s => s.Course)
            .Include(e => e.AcademicTerm)
            .Where(e => e.SectionId == sectionId && e.Status != EnrollmentRecordStatus.Dropped)
            .ToListAsync();

        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentName = $"{e.Student.User.FirstName} {e.Student.User.LastName}",
            SectionId = e.SectionId,
            CourseName = e.Section.Course.Name,
            CourseCode = e.Section.Course.Code,
            AcademicTermId = e.AcademicTermId,
            AcademicTermName = e.AcademicTerm.Name,
            Status = e.Status,
            RegisteredAt = e.RegisteredAt
        });
    }
}
