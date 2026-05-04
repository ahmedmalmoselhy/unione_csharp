using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/faculties")]
public class FacultyController : ControllerBase
{
    private readonly IFacultyService _facultyService;

    public FacultyController(IFacultyService facultyService)
    {
        _facultyService = facultyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FacultyDto>>> GetAll([FromQuery] string? search, [FromQuery] bool? isActive, [FromQuery] string? enrollmentType)
    {
        var faculties = await _facultyService.GetAllFacultiesAsync(search, isActive, enrollmentType);
        return Ok(faculties);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FacultyDto>> GetById(long id)
    {
        try
        {
            var faculty = await _facultyService.GetFacultyByIdAsync(id);
            return Ok(faculty);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<FacultyDto>> Create([FromForm] CreateFacultyDto dto, IFormFile? logo)
    {
        Stream? logoStream = null;
        string? fileName = null;

        if (logo != null)
        {
            logoStream = logo.OpenReadStream();
            fileName = logo.FileName;
        }

        var faculty = await _facultyService.CreateFacultyAsync(dto, logoStream, fileName);
        return CreatedAtAction(nameof(GetById), new { id = faculty.Id }, faculty);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FacultyDto>> Update(long id, [FromForm] UpdateFacultyDto dto, IFormFile? logo)
    {
        Stream? logoStream = null;
        string? fileName = null;

        if (logo != null)
        {
            logoStream = logo.OpenReadStream();
            fileName = logo.FileName;
        }

        try
        {
            var faculty = await _facultyService.UpdateFacultyAsync(id, dto, logoStream, fileName);
            return Ok(faculty);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _facultyService.DeleteFacultyAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }
}
