using UniOne.Domain.Enums;

namespace UniOne.Application.DTOs;

public class AttendanceDto
{
    public long Id { get; set; }
    public long AttendanceSessionId { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
}

public class AttendanceSessionDto
{
    public long Id { get; set; }
    public long SectionId { get; set; }
    public DateTime Date { get; set; }
    public string? Topic { get; set; }
    public List<AttendanceDto> Records { get; set; } = new();
}

public class CreateAttendanceSessionDto
{
    public DateTime Date { get; set; }
    public string? Topic { get; set; }
}

public class RecordAttendanceDto
{
    public long StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
}
