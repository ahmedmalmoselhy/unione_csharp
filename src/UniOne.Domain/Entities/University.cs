namespace UniOne.Domain.Entities;

public class University
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Address { get; set; }
    public string? LogoPath { get; set; }
    public long? PresidentId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateOnly? EstablishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<UniversityVicePresident> VicePresidents { get; set; } = new List<UniversityVicePresident>();
}
