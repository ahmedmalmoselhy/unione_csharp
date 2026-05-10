using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IApplicationDbContext _context;

    public AttendanceService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AttendanceSessionDto>> GetSectionSessions(long sectionId)
    {
        return await _context.AttendanceSessions
            .Where(s => s.SectionId == sectionId)
            .OrderByDescending(s => s.Date)
            .Select(s => new AttendanceSessionDto
            {
                Id = s.Id,
                SectionId = s.SectionId,
                Date = s.Date,
                Topic = s.Topic
            })
            .ToListAsync();
    }

    public async Task<AttendanceSessionDto> GetSession(long sessionId)
    {
        var session = await _context.AttendanceSessions
            .Include(s => s.Records)
            .ThenInclude(r => r.Student)
            .ThenInclude(st => st.User)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null) throw new KeyNotFoundException("Session not found");

        return new AttendanceSessionDto
        {
            Id = session.Id,
            SectionId = session.SectionId,
            Date = session.Date,
            Topic = session.Topic,
            Records = session.Records.Select(r => new AttendanceDto
            {
                Id = r.Id,
                AttendanceSessionId = r.AttendanceSessionId,
                StudentId = r.StudentId,
                StudentName = $"{r.Student.User.FirstName} {r.Student.User.LastName}",
                Status = r.Status,
                Note = r.Note
            }).ToList()
        };
    }

    public async Task<AttendanceSessionDto> CreateSession(long sectionId, CreateAttendanceSessionDto dto)
    {
        var session = new AttendanceSession
        {
            SectionId = sectionId,
            Date = dto.Date,
            Topic = dto.Topic
        };

        _context.AttendanceSessions.Add(session);
        await _context.SaveChangesAsync();

        return new AttendanceSessionDto
        {
            Id = session.Id,
            SectionId = session.SectionId,
            Date = session.Date,
            Topic = session.Topic
        };
    }

    public async Task UpdateRecords(long sessionId, IEnumerable<RecordAttendanceDto> records)
    {
        var existingRecords = await _context.AttendanceRecords
            .Where(r => r.AttendanceSessionId == sessionId)
            .ToDictionaryAsync(r => r.StudentId);

        foreach (var recordDto in records)
        {
            if (existingRecords.TryGetValue(recordDto.StudentId, out var record))
            {
                record.Status = recordDto.Status;
                record.Note = recordDto.Note;
                record.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    AttendanceSessionId = sessionId,
                    StudentId = recordDto.StudentId,
                    Status = recordDto.Status,
                    Note = recordDto.Note
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AttendanceDto>> GetStudentAttendance(long studentId, long? sectionId = null)
    {
        var query = _context.AttendanceRecords
            .Include(r => r.AttendanceSession)
            .Where(r => r.StudentId == studentId);

        if (sectionId.HasValue)
        {
            query = query.Where(r => r.AttendanceSession.SectionId == sectionId.Value);
        }

        return await query
            .OrderByDescending(r => r.AttendanceSession.Date)
            .Select(r => new AttendanceDto
            {
                Id = r.Id,
                AttendanceSessionId = r.AttendanceSessionId,
                StudentId = r.StudentId,
                Status = r.Status,
                Note = r.Note
            })
            .ToListAsync();
    }
}
