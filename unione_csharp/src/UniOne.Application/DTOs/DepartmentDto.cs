using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class DepartmentDto
{
    public long Id { get; set; }
    public long? FacultyId { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public string? LogoPath { get; set; }
    public DepartmentType Type { get; set; }
    public DepartmentScope Scope { get; set; }
    public bool IsPreparatory { get; set; }
    public long? HeadId { get; set; }
    public bool IsActive { get; set; }
    public bool IsMandatory { get; set; }
    public ushort? RequiredCreditHours { get; set; }
}

public class CreateDepartmentDto
{
    public long? FacultyId { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public DepartmentType Type { get; set; }
    public bool IsPreparatory { get; set; }
    public long? HeadId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateDepartmentDto
{
    public long? FacultyId { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public bool IsPreparatory { get; set; }
    public long? HeadId { get; set; }
    public bool IsActive { get; set; }
    public ushort? RequiredCreditHours { get; set; }
    public bool RemoveLogo { get; set; }
}
