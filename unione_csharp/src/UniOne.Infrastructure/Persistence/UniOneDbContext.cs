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
    public DbSet<AcademicTerm> AcademicTerms { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
    public DbSet<DepartmentCourse> DepartmentCourses { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<EnrollmentWaitlist> EnrollmentWaitlists { get; set; }

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
        builder.Entity<AcademicTerm>().ToTable("academic_terms");
        builder.Entity<Course>().ToTable("courses");
        builder.Entity<Section>().ToTable("sections");
        builder.Entity<Enrollment>().ToTable("enrollments");
        builder.Entity<EnrollmentWaitlist>().ToTable("enrollment_waitlist");

        // Many-to-many relationships
        builder.Entity<CoursePrerequisite>()
            .ToTable("course_prerequisites")
            .HasKey(cp => new { cp.CourseId, cp.PrerequisiteId });

        builder.Entity<DepartmentCourse>()
            .ToTable("department_course")
            .HasKey(dc => new { dc.DepartmentId, dc.CourseId });

        builder.Entity<CoursePrerequisite>()
            .HasOne(cp => cp.Course)
            .WithMany(c => c.Prerequisites)
            .HasForeignKey(cp => cp.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CoursePrerequisite>()
            .HasOne(cp => cp.Prerequisite)
            .WithMany(c => c.Dependents)
            .HasForeignKey(cp => cp.PrerequisiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DepartmentCourse>()
            .HasOne(dc => dc.Department)
            .WithMany()
            .HasForeignKey(dc => dc.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DepartmentCourse>()
            .HasOne(dc => dc.Course)
            .WithMany(c => c.DepartmentCourses)
            .HasForeignKey(dc => dc.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Identity-specific renames (optional, but keeps schema cleaner)
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<long>>().ToTable("user_roles_map");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<long>>().ToTable("user_claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<long>>().ToTable("user_logins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>>().ToTable("role_claims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<long>>().ToTable("user_tokens");
    }
}
