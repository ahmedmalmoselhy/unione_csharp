using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Application.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly IApplicationDbContext _context;

    public AnnouncementService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AnnouncementDto>> GetAnnouncements(long userId)
    {
        var user = await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Professor)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Enumerable.Empty<AnnouncementDto>();

        var query = _context.Announcements
            .Include(a => a.Creator)
            .Include(a => a.Reads.Where(r => r.UserId == userId))
            .Where(a => a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow);

        // Filter based on audience
        // This is a simplified filter logic
        var studentId = user.Student?.Id;
        var professorId = user.Professor?.Id;
        var employeeId = user.Employee?.Id;

        query = query.Where(a =>
            a.Audience == AnnouncementAudience.All ||
            (a.Audience == AnnouncementAudience.Students && studentId != null) ||
            (a.Audience == AnnouncementAudience.Professors && professorId != null) ||
            (a.Audience == AnnouncementAudience.Employees && employeeId != null) ||
            (a.Audience == AnnouncementAudience.Faculty && user.Student != null && a.FacultyId == user.Student.FacultyId) ||
            (a.Audience == AnnouncementAudience.Department && user.Student != null && a.DepartmentId == user.Student.DepartmentId)
        );

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Audience = a.Audience,
                FacultyId = a.FacultyId,
                DepartmentId = a.DepartmentId,
                CreatorName = $"{a.Creator.FirstName} {a.Creator.LastName}",
                CreatedAt = a.CreatedAt,
                IsRead = a.Reads.Any()
            })
            .ToListAsync();
    }

    public async Task<AnnouncementDto> CreateAnnouncement(CreateAnnouncementDto dto, long creatorId)
    {
        var announcement = new Announcement
        {
            Title = dto.Title,
            Content = dto.Content,
            Audience = dto.Audience,
            FacultyId = dto.FacultyId,
            DepartmentId = dto.DepartmentId,
            CreatedBy = creatorId,
            ExpiresAt = dto.ExpiresAt
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return new AnnouncementDto
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            Audience = announcement.Audience,
            CreatedAt = announcement.CreatedAt
        };
    }

    public async Task MarkAsRead(long announcementId, long userId)
    {
        if (!await _context.AnnouncementReads.AnyAsync(r => r.AnnouncementId == announcementId && r.UserId == userId))
        {
            _context.AnnouncementReads.Add(new AnnouncementRead
            {
                AnnouncementId = announcementId,
                UserId = userId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SectionAnnouncementDto>> GetSectionAnnouncements(long sectionId)
    {
        return await _context.SectionAnnouncements
            .Include(a => a.Creator)
            .Where(a => a.SectionId == sectionId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new SectionAnnouncementDto
            {
                Id = a.Id,
                SectionId = a.SectionId,
                Title = a.Title,
                Content = a.Content,
                CreatorName = $"{a.Creator.FirstName} {a.Creator.LastName}",
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<SectionAnnouncementDto> CreateSectionAnnouncement(long sectionId, CreateSectionAnnouncementDto dto, long creatorId)
    {
        var announcement = new SectionAnnouncement
        {
            SectionId = sectionId,
            Title = dto.Title,
            Content = dto.Content,
            CreatedBy = creatorId
        };

        _context.SectionAnnouncements.Add(announcement);
        await _context.SaveChangesAsync();

        var creator = await _context.Users.FindAsync(creatorId);

        return new SectionAnnouncementDto
        {
            Id = announcement.Id,
            SectionId = announcement.SectionId,
            Title = announcement.Title,
            Content = announcement.Content,
            CreatorName = creator != null ? $"{creator.FirstName} {creator.LastName}" : "Unknown",
            CreatedAt = announcement.CreatedAt
        };
    }

    public async Task DeleteSectionAnnouncement(long announcementId)
    {
        var announcement = await _context.SectionAnnouncements.FindAsync(announcementId);
        if (announcement != null)
        {
            _context.SectionAnnouncements.Remove(announcement);
            await _context.SaveChangesAsync();
        }
    }
}
