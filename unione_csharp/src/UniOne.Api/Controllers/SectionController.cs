using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/sections")]
public class SectionController : ControllerBase
{
    private readonly IAcademicCatalogService _catalogService;

    public SectionController(IAcademicCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SectionDto>>> GetAll([FromQuery] long? courseId, [FromQuery] long? termId, [FromQuery] long? professorId)
    {
        return Ok(await _catalogService.GetAllSectionsAsync(courseId, termId, professorId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SectionDto>> GetById(long id)
    {
        try { return Ok(await _catalogService.GetSectionByIdAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost]
    public async Task<ActionResult<SectionDto>> Create(CreateSectionDto dto)
    {
        var section = await _catalogService.CreateSectionAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = section.Id }, section);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SectionDto>> Update(long id, UpdateSectionDto dto)
    {
        try { return Ok(await _catalogService.UpdateSectionAsync(id, dto)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _catalogService.DeleteSectionAsync(id); return NoContent(); }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
