using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class EnrollmentDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public long SectionId { get; set; }
    public string CourseName { get; set; } = null!;
    public string CourseCode { get; set; } = null!;
    public long AcademicTermId { get; set; }
    public string AcademicTermName { get; set; } = null!;
    public EnrollmentRecordStatus Status { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? DroppedAt { get; set; }
}

public class CreateEnrollmentDto
{
    public long StudentId { get; set; }
    public long SectionId { get; set; }
}
