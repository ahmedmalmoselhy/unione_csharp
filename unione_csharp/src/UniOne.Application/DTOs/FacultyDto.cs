using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class FacultyDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public string? LogoPath { get; set; }
    public EnrollmentType EnrollmentType { get; set; }
    public long? DeanId { get; set; }
    public bool IsActive { get; set; }
}

public class CreateFacultyDto
{
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public EnrollmentType EnrollmentType { get; set; }
    public long? DeanId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateFacultyDto
{
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public EnrollmentType EnrollmentType { get; set; }
    public long? DeanId { get; set; }
    public bool IsActive { get; set; }
    public bool RemoveLogo { get; set; }
}
