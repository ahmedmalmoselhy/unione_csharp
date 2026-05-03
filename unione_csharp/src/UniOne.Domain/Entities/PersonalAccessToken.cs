namespace UniOne.Domain.Entities;

public class PersonalAccessToken
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public required string Name { get; set; }
    public required string TokenHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
