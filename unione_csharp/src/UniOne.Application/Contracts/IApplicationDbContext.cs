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
    DbSet<Professor> Professors { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Student> Students { get; }
    DbSet<StudentDepartmentHistory> StudentDepartmentHistories { get; }
    DbSet<AcademicTerm> AcademicTerms { get; }
    DbSet<Course> Courses { get; }
    DbSet<CoursePrerequisite> CoursePrerequisites { get; }
    DbSet<DepartmentCourse> DepartmentCourses { get; }
    DbSet<Section> Sections { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<EnrollmentWaitlist> EnrollmentWaitlists { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
