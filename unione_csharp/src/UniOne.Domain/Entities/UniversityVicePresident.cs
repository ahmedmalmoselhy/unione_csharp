namespace UniOne.Domain.Entities;

public class UniversityVicePresident
{
    public long Id { get; set; }
    public long UniversityId { get; set; }
    public long ProfessorId { get; set; }
    public required string Title { get; set; }
    public required string TitleAr { get; set; }
    public byte Order { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly AppointedAt { get; set; }
    public DateOnly? EndedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual University University { get; set; } = null!;
}
