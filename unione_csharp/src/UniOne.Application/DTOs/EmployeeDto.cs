using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class EmployeeDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string StaffNumber { get; set; } = null!;
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public EmploymentType EmploymentType { get; set; }
    public decimal? Salary { get; set; }
    public DateOnly HiredAt { get; set; }
    public DateOnly? TerminatedAt { get; set; }
}

public class CreateEmployeeDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string NationalId { get; set; }
    public string? Phone { get; set; }
    public required string StaffNumber { get; set; }
    public long DepartmentId { get; set; }
    public required string JobTitle { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal? Salary { get; set; }
    public DateOnly HiredAt { get; set; }
}

public class UpdateEmployeeDto
{
    public long DepartmentId { get; set; }
    public required string JobTitle { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal? Salary { get; set; }
    public DateOnly HiredAt { get; set; }
    public DateOnly? TerminatedAt { get; set; }
}
