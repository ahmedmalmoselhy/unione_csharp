using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniOne.Domain.Entities;

namespace UniOne.Infrastructure.Persistence;

public class UniOneDbContext : IdentityDbContext<User, Role, long>
{
    public UniOneDbContext(DbContextOptions<UniOneDbContext> options) : base(options)
    {
    }

    public DbSet<RoleAssignment> RoleAssignments { get; set; }
    public DbSet<PersonalAccessToken> PersonalAccessTokens { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<UniversityVicePresident> UniversityVicePresidents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations from current assembly
        builder.ApplyConfigurationsFromAssembly(typeof(UniOneDbContext).Assembly);

        // Rename Identity tables to match Laravel convention where possible
        builder.Entity<User>().ToTable("users");
        builder.Entity<Role>().ToTable("roles");
        builder.Entity<RoleAssignment>().ToTable("role_user");
        builder.Entity<PersonalAccessToken>().ToTable("personal_access_tokens");
        builder.Entity<University>().ToTable("university");
        builder.Entity<Faculty>().ToTable("faculties");
        builder.Entity<Department>().ToTable("departments");
        builder.Entity<UniversityVicePresident>().ToTable("university_vice_presidents");

        // Identity-specific renames (optional, but keeps schema cleaner)
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<long>>().ToTable("user_roles_map");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<long>>().ToTable("user_claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<long>>().ToTable("user_logins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>>().ToTable("role_claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<long>>().ToTable("user_tokens");
    }
}
