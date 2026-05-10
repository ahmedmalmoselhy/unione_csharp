using UniOne.Application.DTOs;

namespace UniOne.Application.Contracts;

public interface IAnnouncementService
{
    Task<IEnumerable<AnnouncementDto>> GetAnnouncements(long userId);
    Task<AnnouncementDto> CreateAnnouncement(CreateAnnouncementDto dto, long creatorId);
    Task MarkAsRead(long announcementId, long userId);

    Task<IEnumerable<SectionAnnouncementDto>> GetSectionAnnouncements(long sectionId);
    Task<SectionAnnouncementDto> CreateSectionAnnouncement(long sectionId, CreateSectionAnnouncementDto dto, long creatorId);
    Task DeleteSectionAnnouncement(long announcementId);
}
