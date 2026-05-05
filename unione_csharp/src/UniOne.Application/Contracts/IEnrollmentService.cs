using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>> GetStudentEnrollmentsAsync(long studentId);
    Task<EnrollmentDto> EnrollStudentAsync(CreateEnrollmentDto dto);
    Task DropEnrollmentAsync(long enrollmentId);
    Task<IEnumerable<EnrollmentDto>> GetSectionEnrollmentsAsync(long sectionId);
}
