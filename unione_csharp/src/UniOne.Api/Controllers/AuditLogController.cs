using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniOne.Infrastructure.Persistence;

namespace UniOne.Api.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/v1/admin/audit-logs")]
public class AuditLogController : ControllerBase
{
    private readonly UniOneDbContext _context;

    public AuditLogController(UniOneDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? action, [FromQuery] string? auditableType, [FromQuery] long? auditableId)
    {
        var query = _context.AuditLogs
            .Include(al => al.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(al => al.Action == action);
        }

        if (!string.IsNullOrEmpty(auditableType))
        {
            query = query.Where(al => al.AuditableType == auditableType);
        }

        if (auditableId.HasValue)
        {
            query = query.Where(al => al.AuditableId == auditableId.Value);
        }

        var logs = await query.OrderByDescending(al => al.CreatedAt).Take(100).ToListAsync();
        
        return Ok(logs.Select(al => new
        {
            al.Id,
            al.Action,
            al.AuditableType,
            al.AuditableId,
            al.Description,
            al.OldValues,
            al.NewValues,
            al.IpAddress,
            al.CreatedAt,
            User = al.User != null ? new { al.User.Id, al.User.FirstName, al.User.LastName } : null
        }));
    }
}
