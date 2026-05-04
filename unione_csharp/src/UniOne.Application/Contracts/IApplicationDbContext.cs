using Microsoft.EntityFrameworkCore;
using UniOne.Domain.Entities;

namespace UniOne.Application.Contracts;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<RoleAssignment> RoleAssignments { get; }
    DbSet<PersonalAccessToken> PersonalAccessTokens { get; }
    DbSet<University> Universities { get; }
    DbSet<Faculty> Faculties { get; }
    DbSet<Department> Departments { get; }
    DbSet<UniversityVicePresident> UniversityVicePresidents { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
