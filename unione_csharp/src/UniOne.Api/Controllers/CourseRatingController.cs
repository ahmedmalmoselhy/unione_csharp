using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;

namespace UniOne.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1")]
public class CourseRatingController : ControllerBase
{
    private readonly ICourseRatingService _ratingService;
    private readonly ICurrentUserService _currentUserService;

    public CourseRatingController(ICourseRatingService ratingService, ICurrentUserService currentUserService)
    {
        _ratingService = ratingService;
        _currentUserService = currentUserService;
    }

    [HttpGet("student/ratings")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<IEnumerable<CourseRatingDto>>> GetMyRatings([FromQuery] long courseId)
    {
        // For simplicity, we just return all ratings for a course
        // In a real app we might filter to show only student's own ratings or all
        return Ok(await _ratingService.GetCourseRatings(courseId));
    }

    [HttpPost("student/ratings")]
    [Authorize(Policy = "StudentOnly")]
    public async Task<ActionResult<CourseRatingDto>> CreateRating(CreateCourseRatingDto dto)
    {
        var studentId = _currentUserService.StudentId;
        if (!studentId.HasValue) return Forbid();

        var rating = await _ratingService.CreateRating(dto, studentId.Value);
        return Ok(rating);
    }

    [HttpGet("admin/ratings")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<CourseRatingDto>>> GetAllRatings([FromQuery] long courseId)
    {
        return Ok(await _ratingService.GetCourseRatings(courseId));
    }
}
