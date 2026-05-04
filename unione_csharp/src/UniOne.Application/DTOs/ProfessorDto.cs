using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class ProfessorDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string StaffNumber { get; set; } = null!;
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string Specialization { get; set; } = null!;
    public AcademicRank AcademicRank { get; set; }
    public string? OfficeLocation { get; set; }
    public DateOnly HiredAt { get; set; }
}

public class CreateProfessorDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string NationalId { get; set; }
    public string? Phone { get; set; }
    public required string StaffNumber { get; set; }
    public long DepartmentId { get; set; }
    public required string Specialization { get; set; }
    public AcademicRank AcademicRank { get; set; }
    public string? OfficeLocation { get; set; }
    public DateOnly HiredAt { get; set; }
}

public class UpdateProfessorDto
{
    public long DepartmentId { get; set; }
    public required string Specialization { get; set; }
    public AcademicRank AcademicRank { get; set; }
    public string? OfficeLocation { get; set; }
    public DateOnly HiredAt { get; set; }
}
