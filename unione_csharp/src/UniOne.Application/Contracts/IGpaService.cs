using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IGpaService
{
    decimal CalculateGradePoints(string gradeLetter);
    string GetGradeLetter(decimal score);
    Task RecalculateStudentGpa(long studentId);
    Task<StudentTermGpaDto> GetTermGpa(long studentId, long academicTermId);
}
