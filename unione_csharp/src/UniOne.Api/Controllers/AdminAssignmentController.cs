using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/assignments")]
public class AdminAssignmentController : ControllerBase
{
    private readonly IRoleAssignmentService _assignmentService;

    public AdminAssignmentController(IRoleAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleAssignmentDto>>> Get([FromQuery] long? facultyId, [FromQuery] long? departmentId)
    {
        var assignments = await _assignmentService.GetAssignmentsAsync(facultyId, departmentId);
        return Ok(assignments);
    }

    [HttpPost]
    public async Task<IActionResult> Assign([FromBody] AssignRoleDto dto)
    {
        try
        {
            await _assignmentService.AssignRoleAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Revoke(long id)
    {
        try
        {
            await _assignmentService.RevokeRoleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
