namespace UniOne.Application.DTOs;

public class UniversityDto
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
}

public class UpdateUniversityDto
{
    public required string Name { get; set; }
    public required string NameAr { get; set; }
    public required string Address { get; set; }
    public long? PresidentId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateOnly? EstablishedAt { get; set; }
    public bool RemoveLogo { get; set; }
}
