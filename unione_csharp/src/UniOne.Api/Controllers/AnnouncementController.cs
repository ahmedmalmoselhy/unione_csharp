using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1")]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;
    private readonly ICurrentUserService _currentUserService;

    public AnnouncementController(IAnnouncementService announcementService, ICurrentUserService currentUserService)
    {
        _announcementService = announcementService;
        _currentUserService = currentUserService;
    }

    [HttpGet("announcements")]
    public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetAnnouncements()
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        return Ok(await _announcementService.GetAnnouncements(userId.Value));
    }

    [HttpPost("announcements/{id}/read")]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        await _announcementService.MarkAsRead(id, userId.Value);
        return NoContent();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("admin/announcements")]
    public async Task<ActionResult<AnnouncementDto>> CreateAnnouncement(CreateAnnouncementDto dto)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var announcement = await _announcementService.CreateAnnouncement(dto, userId.Value);
        return Ok(announcement);
    }

    [HttpGet("student/sections/{sectionId}/announcements")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<IEnumerable<SectionAnnouncementDto>>> GetSectionAnnouncementsForStudent(long sectionId)
    {
        return Ok(await _announcementService.GetSectionAnnouncements(sectionId));
    }

    [HttpGet("professor/sections/{sectionId}/announcements")]
    [Authorize(Policy = "ProfessorOnly")]
    public async Task<ActionResult<IEnumerable<SectionAnnouncementDto>>> GetSectionAnnouncementsForProfessor(long sectionId)
    {
        return Ok(await _announcementService.GetSectionAnnouncements(sectionId));
    }

    [HttpPost("professor/sections/{sectionId}/announcements")]
    [Authorize(Policy = "ProfessorOnly")]
    public async Task<ActionResult<SectionAnnouncementDto>> CreateSectionAnnouncement(long sectionId, CreateSectionAnnouncementDto dto)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var announcement = await _announcementService.CreateSectionAnnouncement(sectionId, dto, userId.Value);
        return Ok(announcement);
    }

    [HttpDelete("professor/sections/{sectionId}/announcements/{announcementId}")]
    [Authorize(Policy = "ProfessorOnly")]
    public async Task<IActionResult> DeleteSectionAnnouncement(long sectionId, long announcementId)
    {
        await _announcementService.DeleteSectionAnnouncement(announcementId);
        return NoContent();
    }
}
