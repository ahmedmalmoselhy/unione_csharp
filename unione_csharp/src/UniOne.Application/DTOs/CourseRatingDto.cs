namespace UniOne.Application.DTOs;

public class CourseRatingDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string? StudentName { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public long? SectionId { get; set; }
    public byte Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCourseRatingDto
{
    public long CourseId { get; set; }
    public long? SectionId { get; set; }
    public byte Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsAnonymous { get; set; }
}
