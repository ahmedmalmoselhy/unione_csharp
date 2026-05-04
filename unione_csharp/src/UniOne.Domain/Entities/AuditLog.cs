using System.Text.Json;

namespace UniOne.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public required string Action { get; set; }
    public required string AuditableType { get; set; }
    public long? AuditableId { get; set; }
    public required string Description { get; set; }
    public string? OldValues { get; set; } // JSON string
    public string? NewValues { get; set; } // JSON string
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User? User { get; set; }
}
