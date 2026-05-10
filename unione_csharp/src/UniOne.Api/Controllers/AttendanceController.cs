using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly ICurrentUserService _currentUserService;

    public AttendanceController(IAttendanceService attendanceService, ICurrentUserService currentUserService)
    {
        _attendanceService = attendanceService;
        _currentUserService = currentUserService;
    }

    [Authorize(Policy = "ProfessorOnly")]
    [HttpGet("professor/sections/{sectionId}/attendance")]
    public async Task<ActionResult<IEnumerable<AttendanceSessionDto>>> GetSectionSessions(long sectionId)
    {
        return Ok(await _attendanceService.GetSectionSessions(sectionId));
    }

    [Authorize(Policy = "ProfessorOnly")]
    [HttpGet("professor/sections/{sectionId}/attendance/{sessionId}")]
    public async Task<ActionResult<AttendanceSessionDto>> GetSession(long sectionId, long sessionId)
    {
        try
        {
            var session = await _attendanceService.GetSession(sessionId);
            if (session.SectionId != sectionId) return BadRequest("Session does not belong to this section");
            return Ok(session);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Policy = "ProfessorOnly")]
    [HttpPost("professor/sections/{sectionId}/attendance")]
    public async Task<ActionResult<AttendanceSessionDto>> CreateSession(long sectionId, CreateAttendanceSessionDto dto)
    {
        var session = await _attendanceService.CreateSession(sectionId, dto);
        return CreatedAtAction(nameof(GetSession), new { sectionId, sessionId = session.Id }, session);
    }

    [Authorize(Policy = "ProfessorOnly")]
    [HttpPut("professor/sections/{sectionId}/attendance/{sessionId}")]
    public async Task<IActionResult> UpdateRecords(long sectionId, long sessionId, [FromBody] IEnumerable<RecordAttendanceDto> records)
    {
        await _attendanceService.UpdateRecords(sessionId, records);
        return NoContent();
    }

    [Authorize(Policy = "StudentOnly")]
    [HttpGet("student/attendance")]
    public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetStudentAttendance([FromQuery] long? sectionId)
    {
        var studentId = _currentUserService.StudentId;
        if (!studentId.HasValue) return Forbid();

        return Ok(await _attendanceService.GetStudentAttendance(studentId.Value, sectionId));
    }
}
