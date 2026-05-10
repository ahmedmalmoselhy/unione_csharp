using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class Announcement
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public AnnouncementAudience Audience { get; set; }
    public long? FacultyId { get; set; }
    public long? DepartmentId { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Faculty? Faculty { get; set; }
    public virtual Department? Department { get; set; }
    public virtual User Creator { get; set; } = null!;
    public virtual ICollection<AnnouncementRead> Reads { get; set; } = new List<AnnouncementRead>();
}

public class AnnouncementRead
{
    public long Id { get; set; }
    public long AnnouncementId { get; set; }
    public long UserId { get; set; }
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;

    public virtual Announcement Announcement { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
