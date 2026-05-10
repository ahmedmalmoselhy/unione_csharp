using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IAttendanceService
{
    Task<IEnumerable<AttendanceSessionDto>> GetSectionSessions(long sectionId);
    Task<AttendanceSessionDto> GetSession(long sessionId);
    Task<AttendanceSessionDto> CreateSession(long sectionId, CreateAttendanceSessionDto dto);
    Task UpdateRecords(long sessionId, IEnumerable<RecordAttendanceDto> records);
    Task<IEnumerable<AttendanceDto>> GetStudentAttendance(long studentId, long? sectionId = null);
}
