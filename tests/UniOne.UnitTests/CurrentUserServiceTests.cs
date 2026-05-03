using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using UniOne.Infrastructure.Identity;

namespace UniOne.UnitTests;

public class CurrentUserServiceTests
{
    [Fact]
    public void CurrentUserService_ReadsUserRolesAndScopesFromClaims()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "42"),
                    new Claim(ClaimTypes.Role, "faculty_admin"),
                    new Claim("faculty_scope", "7"),
                    new Claim("faculty_scope", "7"),
                    new Claim("department_scope", "12"),
                    new Claim("must_change_password", "true")
                ],
                authenticationType: "Test"))
        };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };

        var currentUser = new CurrentUserService(accessor);

        currentUser.UserId.Should().Be(42);
        currentUser.IsAuthenticated.Should().BeTrue();
        currentUser.Roles.Should().ContainSingle("faculty_admin");
        currentUser.FacultyScopeIds.Should().ContainSingle().Which.Should().Be(7);
        currentUser.DepartmentScopeIds.Should().ContainSingle().Which.Should().Be(12);
        currentUser.MustChangePassword.Should().BeTrue();
    }
}
