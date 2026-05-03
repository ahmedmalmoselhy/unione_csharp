using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniOne.Application.Authorization;
using UniOne.Infrastructure.Persistence;

namespace UniOne.Infrastructure.Identity;

public class ScopedRoleAuthorizationHandler : AuthorizationHandler<ScopedRoleRequirement>
{
    private readonly UniOneDbContext _dbContext;

    public ScopedRoleAuthorizationHandler(UniOneDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopedRoleRequirement requirement)
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var hasActiveAssignment = await _dbContext.RoleAssignments
            .AnyAsync(assignment =>
                assignment.UserId == userId &&
                assignment.RevokedAt == null &&
                requirement.Roles.Contains(assignment.Role.Name!));

        if (hasActiveAssignment)
        {
            context.Succeed(requirement);
        }
    }
}
