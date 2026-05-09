using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1")]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly ICurrentUserService _currentUserService;

    public GradeController(IGradeService gradeService, ICurrentUserService currentUserService)
    {
        _gradeService = gradeService;
        _currentUserService = currentUserService;
    }

    [Authorize(Policy = "ProfessorOnly")]
    [HttpGet("professor/sections/{sectionId}/grades")]
    public async Task<ActionResult<IEnumerable<GradeDto>>> GetSectionGrades(long sectionId)
    {
        var grades = await _gradeService.GetSectionGrades(sectionId);
        return Ok(grades);
    }

    [Authorize(Policy = "ProfessorOnly")]
    [HttpPost("professor/sections/{sectionId}/grades")]
    public async Task<ActionResult<GradeDto>> SubmitGrade(long sectionId, SubmitGradeDto dto)
    {
        try
        {
            var grade = await _gradeService.SubmitGrade(sectionId, dto);
            return Ok(grade);
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("admin/sections/{sectionId}/grades/publish")]
    public async Task<IActionResult> PublishGrades(long sectionId)
    {
        await _gradeService.PublishGrades(sectionId);
        return NoContent();
    }

    [Authorize(Policy = "StudentOnly")]
    [HttpGet("student/grades")]
    public async Task<ActionResult<IEnumerable<GradeDto>>> GetStudentGrades([FromQuery] long? academicTermId)
    {
        var studentId = _currentUserService.StudentId;
        if (!studentId.HasValue) return Forbid();

        return Ok(await _gradeService.GetStudentGrades(studentId.Value, academicTermId));
    }

    [Authorize(Policy = "StudentOnly")]
    [HttpGet("student/gpa")]
    public async Task<ActionResult<StudentTermGpaDto>> GetStudentGpa([FromQuery] long academicTermId, [FromServices] IGpaService gpaService)
    {
        var studentId = _currentUserService.StudentId;
        if (!studentId.HasValue) return Forbid();

        var gpa = await gpaService.GetTermGpa(studentId.Value, academicTermId);
        if (gpa == null) return NotFound();

        return Ok(gpa);
    }

    [Authorize(Policy = "StudentOnly")]
    [HttpGet("student/transcript")]
    public async Task<ActionResult<TranscriptDto>> GetTranscript([FromServices] IStudentService studentService)
    {
        var studentId = _currentUserService.StudentId;
        if (!studentId.HasValue) return Forbid();

        return Ok(await studentService.GetTranscriptAsync(studentId.Value));
    }

    [Authorize(Policy = "StudentOnly")]
    [HttpGet("student/schedule")]
    public async Task<ActionResult<ScheduleDto>> GetSchedule([FromQuery] long? academicTermId, [FromServices] IStudentService studentService)
    {
        var studentId = _currentUserService.StudentId;
        if (!studentId.HasValue) return Forbid();

        return Ok(await studentService.GetScheduleAsync(studentId.Value, academicTermId));
    }
}
