using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Department
{
    public long Id { get; set; }
    public long? FacultyId { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public string? LogoPath { get; set; }
    public DepartmentType Type { get; set; }
    public DepartmentScope Scope { get; set; } = DepartmentScope.Faculty;
    public bool IsPreparatory { get; set; }
    public long? HeadId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMandatory { get; set; }
    public ushort? RequiredCreditHours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Faculty? Faculty { get; set; }
    public virtual User? Head { get; set; }
}
