namespace UniOne.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
