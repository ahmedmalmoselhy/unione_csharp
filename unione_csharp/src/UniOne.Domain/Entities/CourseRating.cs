namespace UniOne.Domain.Entities;

public class CourseRating
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long CourseId { get; set; }
    public long? SectionId { get; set; }
    public byte Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Student Student { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    public virtual Section? Section { get; set; }
}
