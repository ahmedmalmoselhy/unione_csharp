using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/university")]
public class UniversityController : ControllerBase
{
    private readonly IUniversityService _universityService;

    public UniversityController(IUniversityService universityService)
    {
        _universityService = universityService;
    }

    [HttpGet]
    public async Task<ActionResult<UniversityDto>> Get()
    {
        var university = await _universityService.GetUniversityAsync();
        return Ok(university);
    }

    [HttpPut]
    public async Task<ActionResult<UniversityDto>> Update([FromForm] UpdateUniversityDto dto, IFormFile? logo)
    {
        Stream? logoStream = null;
        string? fileName = null;

        if (logo != null)
        {
            logoStream = logo.OpenReadStream();
            fileName = logo.FileName;
        }

        var university = await _universityService.UpdateUniversityAsync(dto, logoStream, fileName);
        return Ok(university);
    }
}
