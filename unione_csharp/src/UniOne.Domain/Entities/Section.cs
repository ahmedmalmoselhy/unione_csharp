namespace UniOne.Domain.Entities;

public class Section
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public long ProfessorId { get; set; }
    public long AcademicTermId { get; set; }
    public ushort Capacity { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; } // JSON string
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Course Course { get; set; } = null!;
    public virtual Professor Professor { get; set; } = null!;
    public virtual AcademicTerm AcademicTerm { get; set; } = null!;
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<EnrollmentWaitlist> Waitlists { get; set; } = new List<EnrollmentWaitlist>();
}
