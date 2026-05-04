using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/professors")]
public class ProfessorController : ControllerBase
{
    private readonly IProfessorService _professorService;

    public ProfessorController(IProfessorService professorService)
    {
        _professorService = professorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetAll([FromQuery] long? departmentId, [FromQuery] string? search)
    {
        var professors = await _professorService.GetAllProfessorsAsync(departmentId, search);
        return Ok(professors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfessorDto>> GetById(long id)
    {
        try
        {
            var professor = await _professorService.GetProfessorByIdAsync(id);
            return Ok(professor);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProfessorDto>> Create([FromBody] CreateProfessorDto dto)
    {
        try
        {
            var professor = await _professorService.CreateProfessorAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = professor.Id }, professor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProfessorDto>> Update(long id, [FromBody] UpdateProfessorDto dto)
    {
        try
        {
            var professor = await _professorService.UpdateProfessorAsync(id, dto);
            return Ok(professor);
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
            await _professorService.DeleteProfessorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] long? departmentId)
    {
        var fileBytes = await _professorService.ExportProfessorsAsync(departmentId);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "professors.xlsx");
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        var result = await _professorService.ImportProfessorsAsync(stream);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Import completed successfully", count = result.Data.Count });
    }
}
