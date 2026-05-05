using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class AcademicTermDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? NameAr { get; set; }
    public required string AcademicYear { get; set; }
    public Semester Semester { get; set; }
    public DateOnly StartsAt { get; set; }
    public DateOnly EndsAt { get; set; }
    public DateTime? RegistrationStartsAt { get; set; }
    public DateTime? RegistrationEndsAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAcademicTermDto
{
    public required string Name { get; set; }
    public string? NameAr { get; set; }
    public required string AcademicYear { get; set; }
    public Semester Semester { get; set; }
    public DateOnly StartsAt { get; set; }
    public DateOnly EndsAt { get; set; }
    public DateTime? RegistrationStartsAt { get; set; }
    public DateTime? RegistrationEndsAt { get; set; }
}

public class UpdateAcademicTermDto : CreateAcademicTermDto
{
    public bool IsActive { get; set; }
}
