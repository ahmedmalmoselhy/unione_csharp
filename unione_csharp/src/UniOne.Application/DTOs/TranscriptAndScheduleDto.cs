namespace UniOne.Application.DTOs;

public class TranscriptDto
{
    public StudentDto Student { get; set; } = null!;
    public decimal CumulativeGpa { get; set; }
    public decimal TotalCreditsEarned { get; set; }
    public List<TranscriptTermDto> Terms { get; set; } = new();
}

public class TranscriptTermDto
{
    public long AcademicTermId { get; set; }
    public string TermName { get; set; } = null!;
    public decimal TermGpa { get; set; }
    public decimal CreditsAttempted { get; set; }
    public decimal CreditsEarned { get; set; }
    public List<TranscriptCourseDto> Courses { get; set; } = new();
}

public class TranscriptCourseDto
{
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public byte CreditHours { get; set; }
    public string GradeLetter { get; set; } = null!;
    public decimal GradePoints { get; set; }
}

public class ScheduleDto
{
    public List<ScheduleItemDto> Items { get; set; } = new();
}

public class ScheduleItemDto
{
    public long SectionId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string CourseName { get; set; } = null!;
    public string SectionNumber { get; set; } = null!;
    public string ProfessorName { get; set; } = null!;
    public object? Schedule { get; set; } // This will be the JSON schedule
}
