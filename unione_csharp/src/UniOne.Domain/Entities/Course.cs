namespace UniOne.Domain.Entities;

public class Course
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public byte CreditHours { get; set; }
    public byte LectureHours { get; set; }
    public byte LabHours { get; set; }
    public byte Level { get; set; }
    public bool IsElective { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();
    public virtual ICollection<CoursePrerequisite> Dependents { get; set; } = new List<CoursePrerequisite>();
    public virtual ICollection<DepartmentCourse> DepartmentCourses { get; set; } = new List<DepartmentCourse>();
}

public class CoursePrerequisite
{
    public long CourseId { get; set; }
    public long PrerequisiteId { get; set; }

    public virtual Course Course { get; set; } = null!;
    public virtual Course Prerequisite { get; set; } = null!;
}

public class DepartmentCourse
{
    public long DepartmentId { get; set; }
    public long CourseId { get; set; }
    public bool IsOwner { get; set; }

    public virtual Department Department { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}
