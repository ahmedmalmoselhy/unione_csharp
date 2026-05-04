using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll([FromQuery] long? facultyId, [FromQuery] long? departmentId, [FromQuery] string? search)
    {
        var students = await _studentService.GetAllStudentsAsync(facultyId, departmentId, search);
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetById(long id)
    {
        try
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            return Ok(student);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto)
    {
        try
        {
            var student = await _studentService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StudentDto>> Update(long id, [FromBody] UpdateStudentDto dto)
    {
        try
        {
            var student = await _studentService.UpdateStudentAsync(id, dto);
            return Ok(student);
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
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/transfer")]
    public async Task<IActionResult> Transfer(long id, [FromBody] TransferStudentDto dto)
    {
        try
        {
            await _studentService.TransferStudentAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
