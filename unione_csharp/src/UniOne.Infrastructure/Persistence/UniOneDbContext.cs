using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniOne.Application.Contracts;
using UniOne.Domain.Entities;

namespace UniOne.Infrastructure.Persistence;

public class UniOneDbContext : IdentityDbContext<User, Role, long>, IApplicationDbContext
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
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Professor> Professors { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentDepartmentHistory> StudentDepartmentHistories { get; set; }

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
        builder.Entity<AuditLog>().ToTable("audit_logs");
        builder.Entity<Professor>().ToTable("professors");
        builder.Entity<Employee>().ToTable("employees");
        builder.Entity<Student>().ToTable("students");
        builder.Entity<StudentDepartmentHistory>().ToTable("student_department_history");

        // Identity-specific renames (optional, but keeps schema cleaner)
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<long>>().ToTable("user_roles_map");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<long>>().ToTable("user_claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<long>>().ToTable("user_logins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>>().ToTable("role_claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<long>>().ToTable("user_tokens");
    }
}
