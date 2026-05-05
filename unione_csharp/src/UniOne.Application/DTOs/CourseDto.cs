namespace UniOne.Application.DTOs;

public class CourseDto
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public byte CreditHours { get; set; }
    public byte Level { get; set; }
    public bool IsElective { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCourseDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public byte CreditHours { get; set; }
    public byte LectureHours { get; set; }
    public byte LabHours { get; set; }
    public byte Level { get; set; }
    public bool IsElective { get; set; }
}

public class UpdateCourseDto : CreateCourseDto
{
    public bool IsActive { get; set; }
}
