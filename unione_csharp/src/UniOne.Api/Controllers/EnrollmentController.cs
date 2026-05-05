using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/enrollments")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByStudent(long studentId)
    {
        return Ok(await _enrollmentService.GetStudentEnrollmentsAsync(studentId));
    }

    [HttpGet("section/{sectionId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetBySection(long sectionId)
    {
        return Ok(await _enrollmentService.GetSectionEnrollmentsAsync(sectionId));
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Enroll(CreateEnrollmentDto dto)
    {
        try
        {
            var enrollment = await _enrollmentService.EnrollStudentAsync(dto);
            return CreatedAtAction(nameof(GetByStudent), new { studentId = dto.StudentId }, enrollment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Drop(long id)
    {
        try
        {
            await _enrollmentService.DropEnrollmentAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }
}
