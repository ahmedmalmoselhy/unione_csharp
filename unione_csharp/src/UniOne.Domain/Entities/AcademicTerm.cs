using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class AcademicTerm
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? NameAr { get; set; }
    public required string AcademicYear { get; set; }
    public Semester Semester { get; set; }
    public DateOnly StartsAt { get; set; }
    public DateOnly EndsAt { get; set; }
    public DateTime? RegistrationStartsAt { get; set; }
    public DateTime? RegistrationEndsAt { get; set; }
    public DateTime? WithdrawalDeadline { get; set; }
    public DateOnly? ExamStartsAt { get; set; }
    public DateOnly? ExamEndsAt { get; set; }
    public DateTime? GradeSubmissionDeadline { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
