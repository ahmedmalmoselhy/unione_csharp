using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IAcademicCatalogService
{
    // Academic Terms
    Task<IEnumerable<AcademicTermDto>> GetAllTermsAsync();
    Task<AcademicTermDto> GetTermByIdAsync(long id);
    Task<AcademicTermDto> CreateTermAsync(CreateAcademicTermDto dto);
    Task<AcademicTermDto> UpdateTermAsync(long id, UpdateAcademicTermDto dto);
    Task DeleteTermAsync(long id);

    // Courses
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync(string? search = null);
    Task<CourseDto> GetCourseByIdAsync(long id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto dto);
    Task DeleteCourseAsync(long id);

    // Sections
    Task<IEnumerable<SectionDto>> GetAllSectionsAsync(long? courseId = null, long? termId = null, long? professorId = null);
    Task<SectionDto> GetSectionByIdAsync(long id);
    Task<SectionDto> CreateSectionAsync(CreateSectionDto dto);
    Task<SectionDto> UpdateSectionAsync(long id, UpdateSectionDto dto);
    Task DeleteSectionAsync(long id);
}
