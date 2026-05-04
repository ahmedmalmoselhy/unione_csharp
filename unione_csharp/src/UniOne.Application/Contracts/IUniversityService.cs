using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IUniversityService
{
    Task<UniversityDto> GetUniversityAsync();
    Task<UniversityDto> UpdateUniversityAsync(UpdateUniversityDto dto, Stream? logoStream, string? logoFileName);
}
