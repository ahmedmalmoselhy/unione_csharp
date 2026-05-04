using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Application.DTOs;
using UniOne.Domain.Entities;

namespace UniOne.Application.Services;

public class RoleAssignmentService : IRoleAssignmentService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAuditLogService _auditLog;

    public RoleAssignmentService(
        IApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IAuditLogService auditLog)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _auditLog = auditLog;
    }

    public async Task<IEnumerable<RoleAssignmentDto>> GetAssignmentsAsync(long? facultyId = null, long? departmentId = null)
    {
        var query = _context.RoleAssignments
            .Include(ra => ra.User)
            .Include(ra => ra.Role)
            .Include(ra => ra.Faculty)
            .Include(ra => ra.Department)
            .Where(ra => ra.RevokedAt == null);

        if (facultyId.HasValue)
        {
            query = query.Where(ra => ra.FacultyId == facultyId.Value);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(ra => ra.DepartmentId == departmentId.Value);
        }

        var assignments = await query.ToListAsync();

        return assignments.Select(ra => new RoleAssignmentDto
        {
            Id = ra.Id,
            UserId = ra.UserId,
            UserFullName = $"{ra.User.FirstName} {ra.User.LastName}",
            RoleId = ra.RoleId,
            RoleName = ra.Role.Name!,
            FacultyId = ra.FacultyId,
            FacultyName = ra.Faculty?.Name,
            DepartmentId = ra.DepartmentId,
            DepartmentName = ra.Department?.Name,
            GrantedAt = ra.GrantedAt,
            RevokedAt = ra.RevokedAt
        });
    }

    public async Task AssignRoleAsync(AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null) throw new KeyNotFoundException("User not found");

        var role = await _roleManager.FindByNameAsync(dto.RoleName);
        if (role == null) throw new KeyNotFoundException("Role not found");

        var existing = await _context.RoleAssignments
            .FirstOrDefaultAsync(ra => ra.UserId == dto.UserId && ra.RoleId == role.Id && 
                                      ra.FacultyId == dto.FacultyId && ra.DepartmentId == dto.DepartmentId &&
                                      ra.RevokedAt == null);

        if (existing != null) return;

        var assignment = new RoleAssignment
        {
            UserId = dto.UserId,
            RoleId = role.Id,
            FacultyId = dto.FacultyId,
            DepartmentId = dto.DepartmentId,
            GrantedAt = DateTime.UtcNow
        };

        _context.RoleAssignments.Add(assignment);
        
        // Also add to Identity user roles if not already there
        if (!await _userManager.IsInRoleAsync(user, dto.RoleName))
        {
            await _userManager.AddToRoleAsync(user, dto.RoleName);
        }

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "created",
            auditableType: "RoleAssignment",
            auditableId: assignment.Id,
            description: $"Assigned role {dto.RoleName} to user {user.FirstName} {user.LastName}",
            newValues: new { dto.UserId, dto.RoleName, dto.FacultyId, dto.DepartmentId });
    }

    public async Task RevokeRoleAsync(long assignmentId)
    {
        var assignment = await _context.RoleAssignments
            .Include(ra => ra.User)
            .Include(ra => ra.Role)
            .FirstOrDefaultAsync(ra => ra.Id == assignmentId);

        if (assignment == null) throw new KeyNotFoundException("Assignment not found");

        assignment.RevokedAt = DateTime.UtcNow;

        // Check if user has other active assignments for the same role
        var otherActive = await _context.RoleAssignments
            .AnyAsync(ra => ra.UserId == assignment.UserId && ra.RoleId == assignment.RoleId && ra.RevokedAt == null && ra.Id != assignmentId);

        if (!otherActive)
        {
            await _userManager.RemoveFromRoleAsync(assignment.User, assignment.Role.Name!);
        }

        await _context.SaveChangesAsync();

        await _auditLog.RecordAsync(
            action: "revoked",
            auditableType: "RoleAssignment",
            auditableId: assignmentId,
            description: $"Revoked role {assignment.Role.Name} from user {assignment.User.FirstName} {assignment.User.LastName}");
    }
}
