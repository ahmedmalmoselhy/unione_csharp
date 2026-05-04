namespace UniOne.Domain.Entities;

public class StudentDepartmentHistory
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long? FromDepartmentId { get; set; }
    public long ToDepartmentId { get; set; }
    public DateTime SwitchedAt { get; set; } = DateTime.UtcNow;
    public long SwitchedBy { get; set; }
    public string? Note { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual Department? FromDepartment { get; set; }
    public virtual Department ToDepartment { get; set; } = null!;
    public virtual User SwitchedByUser { get; set; } = null!;
}
