namespace UniOne.Domain.Entities;

public class SectionAnnouncement
{
    public long Id { get; set; }
    public long SectionId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Section Section { get; set; } = null!;
    public virtual User Creator { get; set; } = null!;
}
