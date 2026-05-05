using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/catalog")]
public class AcademicCatalogController : ControllerBase
{
    private readonly IAcademicCatalogService _catalogService;

    public AcademicCatalogController(IAcademicCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    // Terms
    [HttpGet("terms")]
    public async Task<ActionResult<IEnumerable<AcademicTermDto>>> GetTerms()
    {
        return Ok(await _catalogService.GetAllTermsAsync());
    }

    [HttpGet("terms/{id}")]
    public async Task<ActionResult<AcademicTermDto>> GetTerm(long id)
    {
        try { return Ok(await _catalogService.GetTermByIdAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("terms")]
    public async Task<ActionResult<AcademicTermDto>> CreateTerm(CreateAcademicTermDto dto)
    {
        var term = await _catalogService.CreateTermAsync(dto);
        return CreatedAtAction(nameof(GetTerm), new { id = term.Id }, term);
    }

    [HttpPut("terms/{id}")]
    public async Task<ActionResult<AcademicTermDto>> UpdateTerm(long id, UpdateAcademicTermDto dto)
    {
        try { return Ok(await _catalogService.UpdateTermAsync(id, dto)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("terms/{id}")]
    public async Task<IActionResult> DeleteTerm(long id)
    {
        try { await _catalogService.DeleteTermAsync(id); return NoContent(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Courses
    [HttpGet("courses")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses([FromQuery] string? search)
    {
        return Ok(await _catalogService.GetAllCoursesAsync(search));
    }

    [HttpGet("courses/{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(long id)
    {
        try { return Ok(await _catalogService.GetCourseByIdAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("courses")]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto dto)
    {
        var course = await _catalogService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    [HttpPut("courses/{id}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(long id, UpdateCourseDto dto)
    {
        try { return Ok(await _catalogService.UpdateCourseAsync(id, dto)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("courses/{id}")]
    public async Task<IActionResult> DeleteCourse(long id)
    {
        try { await _catalogService.DeleteCourseAsync(id); return NoContent(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
