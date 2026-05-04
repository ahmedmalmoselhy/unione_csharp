using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Student
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string StudentNumber { get; set; }
    public long FacultyId { get; set; }
    public long? DepartmentId { get; set; }
    public byte AcademicYear { get; set; } = 1;
    public Semester Semester { get; set; } = Semester.First;
    public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Active;
    public AcademicStanding? AcademicStanding { get; set; }
    public decimal? Gpa { get; set; }
    public DateOnly EnrolledAt { get; set; }
    public DateOnly? GraduatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual Department? Department { get; set; }
    public virtual ICollection<StudentDepartmentHistory> DepartmentHistory { get; set; } = new List<StudentDepartmentHistory>();
}
