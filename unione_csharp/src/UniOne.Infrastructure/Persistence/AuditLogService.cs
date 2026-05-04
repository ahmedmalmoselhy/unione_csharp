using System.Text.Json;
using Microsoft.AspNetCore.Http;
using UniOne.Application.Contracts;
using UniOne.Domain.Entities;

namespace UniOne.Infrastructure.Persistence;

public class AuditLogService : IAuditLogService
{
    private readonly UniOneDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(
        UniOneDbContext context,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RecordAsync(
        string action,
        string auditableType,
        long? auditableId,
        string description,
        object? oldValues = null,
        object? newValues = null)
    {
        var auditLog = new AuditLog
        {
            UserId = _currentUserService.UserId,
            Action = action,
            AuditableType = auditableType,
            AuditableId = auditableId,
            Description = description,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
