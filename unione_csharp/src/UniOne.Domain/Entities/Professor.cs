using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Professor
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string StaffNumber { get; set; }
    public long DepartmentId { get; set; }
    public required string Specialization { get; set; }
    public AcademicRank AcademicRank { get; set; }
    public string? OfficeLocation { get; set; }
    public DateOnly HiredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Department Department { get; set; } = null!;
}
