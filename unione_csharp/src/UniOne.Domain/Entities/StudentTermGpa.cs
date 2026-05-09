using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class StudentTermGpa
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long AcademicTermId { get; set; }
    public decimal TermGpa { get; set; }
    public decimal CumulativeGpa { get; set; }
    public decimal CreditsAttempted { get; set; }
    public decimal CreditsEarned { get; set; }
    public AcademicStanding AcademicStanding { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Student Student { get; set; } = null!;
    public virtual AcademicTerm AcademicTerm { get; set; } = null!;
}
