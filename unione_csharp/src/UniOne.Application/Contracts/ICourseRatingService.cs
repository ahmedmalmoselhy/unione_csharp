using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface ICourseRatingService
{
    Task<IEnumerable<CourseRatingDto>> GetCourseRatings(long courseId);
    Task<CourseRatingDto> CreateRating(CreateCourseRatingDto dto, long studentId);
}
