using UniOne.Domain.Enums;

namespace UniOne.Domain.Entities;

public class AttendanceRecord
{
    public long Id { get; set; }
    public long AttendanceSessionId { get; set; }
    public long StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual AttendanceSession AttendanceSession { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
