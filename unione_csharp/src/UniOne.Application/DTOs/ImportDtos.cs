namespace UniOne.Application.DTOs;

public class StudentImportRow
{
    public string NationalId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Gender { get; set; } = null!;
    public string? DateOfBirth { get; set; }
    public string StudentNumber { get; set; } = null!;
    public string? Faculty { get; set; }
    public string? Department { get; set; }
    public int AcademicYear { get; set; }
    public string Semester { get; set; } = null!;
    public string? EnrollmentStatus { get; set; }
}

public class ProfessorImportRow
{
    public string NationalId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Gender { get; set; } = null!;
    public string? DateOfBirth { get; set; }
    public string StaffNumber { get; set; } = null!;
    public string? Faculty { get; set; }
    public string? Department { get; set; }
    public string Specialization { get; set; } = null!;
    public string AcademicRank { get; set; } = null!;
    public string? OfficeLocation { get; set; }
    public string HiredAt { get; set; } = null!;
}

public class EmployeeImportRow
{
    public string NationalId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Gender { get; set; } = null!;
    public string? DateOfBirth { get; set; }
    public string StaffNumber { get; set; } = null!;
    public string? Faculty { get; set; }
    public string? Department { get; set; }
    public string JobTitle { get; set; } = null!;
    public string EmploymentType { get; set; } = null!;
    public decimal? Salary { get; set; }
    public string HiredAt { get; set; } = null!;
}
