namespace UniOne.Domain.Entities;

public class AttendanceSession
{
    public long Id { get; set; }
    public long SectionId { get; set; }
    public DateTime Date { get; set; }
    public string? Topic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Section Section { get; set; } = null!;
    public virtual ICollection<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
}
