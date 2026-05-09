using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IGradeService
{
    Task<IEnumerable<GradeDto>> GetSectionGrades(long sectionId);
    Task<GradeDto> SubmitGrade(long sectionId, SubmitGradeDto dto);
    Task PublishGrades(long sectionId);
    Task<IEnumerable<GradeDto>> GetStudentGrades(long studentId, long? academicTermId = null);
}
