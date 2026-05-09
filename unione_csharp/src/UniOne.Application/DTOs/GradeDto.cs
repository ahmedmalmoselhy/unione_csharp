using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class GradeDto
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
}

public class SubmitGradeDto
{
    public long EnrollmentId { get; set; }
    public decimal Score { get; set; }
}

public class StudentTermGpaDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long AcademicTermId { get; set; }
    public decimal TermGpa { get; set; }
    public decimal CumulativeGpa { get; set; }
    public decimal CreditsAttempted { get; set; }
    public decimal CreditsEarned { get; set; }
    public AcademicStanding AcademicStanding { get; set; }
    public DateTime CalculatedAt { get; set; }
}
