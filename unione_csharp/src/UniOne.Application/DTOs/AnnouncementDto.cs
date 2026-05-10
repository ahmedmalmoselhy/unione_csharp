using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class AnnouncementDto
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public AnnouncementAudience Audience { get; set; }
    public long? FacultyId { get; set; }
    public string? FacultyName { get; set; }
    public long? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string CreatorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class CreateAnnouncementDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public AnnouncementAudience Audience { get; set; }
    public long? FacultyId { get; set; }
    public long? DepartmentId { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class SectionAnnouncementDto
{
    public long Id { get; set; }
    public long SectionId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string CreatorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CreateSectionAnnouncementDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}
