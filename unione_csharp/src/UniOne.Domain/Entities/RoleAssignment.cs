namespace UniOne.Domain.Entities;

public class RoleAssignment
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public long? FacultyId { get; set; }
    public long? DepartmentId { get; set; }
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual Faculty? Faculty { get; set; }
    public virtual Department? Department { get; set; }
}
