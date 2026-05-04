using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Application.Mapping;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class UniversityService : IUniversityService
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IAuditLogService _auditLog;
    private readonly OrganizationMapper _mapper;

    public UniversityService(
        IApplicationDbContext context,
        IFileService fileService,
        IAuditLogService auditLog)
    {
        _context = context;
        _fileService = fileService;
        _auditLog = auditLog;
        _mapper = new OrganizationMapper();
    }

    public async Task<UniversityDto> GetUniversityAsync()
    {
        var university = await _context.Universities.FirstOrDefaultAsync();
        if (university == null)
        {
            throw new InvalidOperationException("University not found.");
        }

        return _mapper.ToDto(university);
    }

    public async Task<UniversityDto> UpdateUniversityAsync(UpdateUniversityDto dto, Stream? logoStream, string? logoFileName)
    {
        var university = await _context.Universities.FirstOrDefaultAsync();
        if (university == null)
        {
            throw new InvalidOperationException("University not found.");
        }

        var oldValues = new
        {
            university.Name,
            university.NameAr,
            university.Address,
            university.PresidentId
        };

        if (dto.RemoveLogo && university.LogoPath != null)
        {
            _fileService.DeleteFile(university.LogoPath);
            university.LogoPath = null;
        }

        if (logoStream != null && logoFileName != null)
        {
            if (university.LogoPath != null)
            {
                _fileService.DeleteFile(university.LogoPath);
            }
            university.LogoPath = await _fileService.SaveFileAsync(logoStream, logoFileName, "university");
        }

        _mapper.UpdateUniversity(dto, university);
        university.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "updated",
            auditableType: "University",
            auditableId: university.Id,
            description: "Updated university information",
            oldValues: oldValues,
            newValues: new
            {
                dto.Name,
                dto.NameAr,
                dto.Address,
                dto.PresidentId
            });

        return _mapper.ToDto(university);
    }
}
