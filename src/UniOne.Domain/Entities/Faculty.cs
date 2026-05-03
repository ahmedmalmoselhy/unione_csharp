using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Faculty
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Code { get; set; }
    public string? LogoPath { get; set; }
    public EnrollmentType EnrollmentType { get; set; }
    public long? DeanId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User? Dean { get; set; }
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
