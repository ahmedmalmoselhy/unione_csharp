using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class CourseRatingService : ICourseRatingService
{
    private readonly IApplicationDbContext _context;

    public CourseRatingService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseRatingDto>> GetCourseRatings(long courseId)
    {
        return await _context.CourseRatings
            .Include(r => r.Student)
            .ThenInclude(s => s.User)
            .Include(r => r.Course)
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CourseRatingDto
            {
                Id = r.Id,
                StudentId = r.StudentId,
                StudentName = r.IsAnonymous ? "Anonymous" : $"{r.Student.User.FirstName} {r.Student.User.LastName}",
                CourseId = r.CourseId,
                CourseName = r.Course.Name,
                SectionId = r.SectionId,
                Rating = r.Rating,
                Comment = r.Comment,
                IsAnonymous = r.IsAnonymous,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CourseRatingDto> CreateRating(CreateCourseRatingDto dto, long studentId)
    {
        var rating = new CourseRating
        {
            StudentId = studentId,
            CourseId = dto.CourseId,
            SectionId = dto.SectionId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            IsAnonymous = dto.IsAnonymous
        };

        _context.CourseRatings.Add(rating);
        await _context.SaveChangesAsync();

        var course = await _context.Courses.FindAsync(dto.CourseId);

        return new CourseRatingDto
        {
            Id = rating.Id,
            StudentId = rating.StudentId,
            CourseId = rating.CourseId,
            CourseName = course?.Name ?? "Unknown",
            SectionId = rating.SectionId,
            Rating = rating.Rating,
            Comment = rating.Comment,
            IsAnonymous = rating.IsAnonymous,
            CreatedAt = rating.CreatedAt
        };
    }
}
