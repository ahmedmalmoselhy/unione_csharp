using Microsoft.AspNetCore.Identity;

namespace UniOne.Domain.Entities;

public class Role : IdentityRole<long>
{
    public required string Label { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<RoleAssignment> RoleAssignments { get; set; } = new List<RoleAssignment>();
}
