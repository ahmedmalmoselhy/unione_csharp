using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "StudentOnly")]
[ApiController]
[Route("api/v1/student/enrollments")]
public class StudentEnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;

    public StudentEnrollmentController(
        IEnrollmentService enrollmentService,
        ICurrentUserService currentUserService,
        IApplicationDbContext context)
    {
        _enrollmentService = enrollmentService;
        _currentUserService = currentUserService;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Store([FromBody] SectionIdRequest request)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId.Value);
        if (student == null) return NotFound("Student record not found.");

        try
        {
            var enrollment = await _enrollmentService.EnrollStudentAsync(new CreateEnrollmentDto
            {
                StudentId = student.Id,
                SectionId = request.SectionId
            });
            return CreatedAtAction(nameof(Index), enrollment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Destroy(long id)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId.Value);
        if (student == null) return NotFound("Student record not found.");

        try
        {
            await _enrollmentService.DropEnrollmentAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> Index()
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId.Value);
        if (student == null) return NotFound("Student record not found.");

        return Ok(await _enrollmentService.GetStudentEnrollmentsAsync(student.Id));
    }
}

public class SectionIdRequest
{
    public long SectionId { get; set; }
}
