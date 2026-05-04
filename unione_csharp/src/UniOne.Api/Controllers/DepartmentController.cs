using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/departments")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll(
        [FromQuery] long? facultyId,
        [FromQuery] string? search,
        [FromQuery] string? type,
        [FromQuery] bool? isActive,
        [FromQuery] bool? noHead)
    {
        var departments = await _departmentService.GetAllDepartmentsAsync(facultyId, search, type, isActive, noHead);
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetById(long id)
    {
        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return Ok(department);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromForm] CreateDepartmentDto dto, IFormFile? logo)
    {
        Stream? logoStream = null;
        string? fileName = null;

        if (logo != null)
        {
            logoStream = logo.OpenReadStream();
            fileName = logo.FileName;
        }

        var department = await _departmentService.CreateDepartmentAsync(dto, logoStream, fileName);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentDto>> Update(long id, [FromForm] UpdateDepartmentDto dto, IFormFile? logo)
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
            var department = await _departmentService.UpdateDepartmentAsync(id, dto, logoStream, fileName);
            return Ok(department);
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
            await _departmentService.DeleteDepartmentAsync(id);
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
