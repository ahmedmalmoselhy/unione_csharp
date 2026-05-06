using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Enrollment
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long SectionId { get; set; }
    public long AcademicTermId { get; set; }
    public EnrollmentRecordStatus Status { get; set; } = EnrollmentRecordStatus.Registered;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? DroppedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Student Student { get; set; } = null!;
    public virtual Section Section { get; set; } = null!;
    public virtual AcademicTerm AcademicTerm { get; set; } = null!;
}

public class EnrollmentWaitlist
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long SectionId { get; set; }
    public long AcademicTermId { get; set; }
    public ushort Position { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Student Student { get; set; } = null!;
    public virtual Section Section { get; set; } = null!;
    public virtual AcademicTerm AcademicTerm { get; set; } = null!;
}
