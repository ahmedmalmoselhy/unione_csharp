using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Employee
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string StaffNumber { get; set; }
    public long DepartmentId { get; set; }
    public required string JobTitle { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public decimal? Salary { get; set; }
    public DateOnly HiredAt { get; set; }
    public DateOnly? TerminatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual Department Department { get; set; } = null!;
}
