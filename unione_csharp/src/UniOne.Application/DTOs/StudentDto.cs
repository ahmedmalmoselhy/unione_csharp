using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class StudentDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string StudentNumber { get; set; } = null!;
    public long FacultyId { get; set; }
    public string FacultyName { get; set; } = null!;
    public long? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public byte AcademicYear { get; set; }
    public Semester Semester { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public AcademicStanding? AcademicStanding { get; set; }
    public decimal? Gpa { get; set; }
    public DateOnly EnrolledAt { get; set; }
    public DateOnly? GraduatedAt { get; set; }
}

public class CreateStudentDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string NationalId { get; set; }
    public string? Phone { get; set; }
    public required string StudentNumber { get; set; }
    public long FacultyId { get; set; }
    public long? DepartmentId { get; set; }
    public byte AcademicYear { get; set; } = 1;
    public Semester Semester { get; set; } = Semester.First;
    public DateOnly EnrolledAt { get; set; }
}

public class UpdateStudentDto
{
    public long? DepartmentId { get; set; }
    public byte AcademicYear { get; set; }
    public Semester Semester { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public AcademicStanding? AcademicStanding { get; set; }
    public decimal? Gpa { get; set; }
    public DateOnly? GraduatedAt { get; set; }
}

public class TransferStudentDto
{
    public long ToDepartmentId { get; set; }
    public string? Note { get; set; }
}
