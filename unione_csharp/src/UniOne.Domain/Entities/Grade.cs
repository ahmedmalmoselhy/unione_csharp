namespace UniOne.Domain.Entities;

public class Grade
{
    public long Id { get; set; }
    public long EnrollmentId { get; set; }
    public long StudentId { get; set; }
    public long SectionId { get; set; }
    public decimal Score { get; set; }
    public string GradeLetter { get; set; } = null!;
    public decimal GradePoints { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Enrollment Enrollment { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual Section Section { get; set; } = null!;
}
