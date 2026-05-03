using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UniOne.Application.Contracts;

namespace UniOne.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(id) ? null : long.Parse(id);
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

    public IEnumerable<long> FacultyScopeIds => GetLongClaims("faculty_scope");

    public IEnumerable<long> DepartmentScopeIds => GetLongClaims("department_scope");

    public bool MustChangePassword
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("must_change_password");
            return claim != null && bool.Parse(claim);
        }
    }

    private IEnumerable<long> GetLongClaims(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?
            .FindAll(claimType)
            .Select(claim => long.TryParse(claim.Value, out var value) ? value : (long?)null)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .Distinct()
            ?? Enumerable.Empty<long>();
    }
}
