using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IProfessorService
{
    Task<IEnumerable<ProfessorDto>> GetAllProfessorsAsync(long? departmentId = null, string? search = null);
    Task<ProfessorDto> GetProfessorByIdAsync(long id);
    Task<ProfessorDto> CreateProfessorAsync(CreateProfessorDto dto);
    Task<ProfessorDto> UpdateProfessorAsync(long id, UpdateProfessorDto dto);
    Task DeleteProfessorAsync(long id);
    Task<byte[]> ExportProfessorsAsync(long? departmentId = null);
    Task<ImportResult<ProfessorImportRow>> ImportProfessorsAsync(Stream fileStream);
}
