using Microsoft.AspNetCore.Authorization;

namespace UniOne.Application.Authorization;

public class ScopedRoleRequirement : IAuthorizationRequirement
{
    public ScopedRoleRequirement(params string[] roles)
    {
        Roles = roles;
    }

    public IReadOnlyCollection<string> Roles { get; }
}
