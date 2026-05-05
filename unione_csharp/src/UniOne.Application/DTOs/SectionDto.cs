namespace UniOne.Application.DTOs;

public class SectionDto
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public string CourseCode { get; set; } = null!;
    public long ProfessorId { get; set; }
    public string ProfessorFullName { get; set; } = null!;
    public long AcademicTermId { get; set; }
    public string AcademicTermName { get; set; } = null!;
    public ushort Capacity { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSectionDto
{
    public long CourseId { get; set; }
    public long ProfessorId { get; set; }
    public long AcademicTermId { get; set; }
    public ushort Capacity { get; set; }
    public string? Room { get; set; }
    public string? Schedule { get; set; }
}

public class UpdateSectionDto : CreateSectionDto
{
    public bool IsActive { get; set; }
}
