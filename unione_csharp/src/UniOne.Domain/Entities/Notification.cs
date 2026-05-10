namespace UniOne.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
    public long UserId { get; set; }
    public required string Data { get; set; } // JSON
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
}
