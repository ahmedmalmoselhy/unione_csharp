using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Infrastructure.Persistence.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.HasQueryFilter(p => p.User.DeletedAt == null);

        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasIndex(p => p.StaffNumber).IsUnique();
        builder.HasIndex(p => p.DepartmentId);

        builder.Property(p => p.StaffNumber).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Specialization).IsRequired().HasMaxLength(255);
        builder.Property(p => p.OfficeLocation).HasMaxLength(255);
        builder.Property(p => p.AcademicRank)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<AcademicRank>(v, true))
            .HasMaxLength(32);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Department)
            .WithMany()
            .HasForeignKey(p => p.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasQueryFilter(e => e.User.DeletedAt == null);

        builder.HasIndex(e => e.UserId).IsUnique();
        builder.HasIndex(e => e.StaffNumber).IsUnique();
        builder.HasIndex(e => e.DepartmentId);

        builder.Property(e => e.StaffNumber).IsRequired().HasMaxLength(255);
        builder.Property(e => e.JobTitle).IsRequired().HasMaxLength(255);
        builder.Property(e => e.EmploymentType)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<EmploymentType>(v, true))
            .HasMaxLength(32);
        builder.Property(e => e.Salary).HasPrecision(10, 2);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Department)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasQueryFilter(s => s.User.DeletedAt == null);

        builder.HasIndex(s => s.UserId).IsUnique();
        builder.HasIndex(s => s.StudentNumber).IsUnique();
        builder.HasIndex(s => s.FacultyId);
        builder.HasIndex(s => s.DepartmentId);

        builder.Property(s => s.StudentNumber).IsRequired().HasMaxLength(255);
        builder.Property(s => s.Semester)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<Semester>(v, true))
            .HasMaxLength(32);
        builder.Property(s => s.EnrollmentStatus)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<EnrollmentStatus>(v, true))
            .HasMaxLength(32);
        builder.Property(s => s.AcademicStanding)
            .HasConversion(
                v => v == null ? null : v.ToString()!.ToLowerInvariant(),
                v => v == null ? null : Enum.Parse<AcademicStanding>(v, true))
            .HasMaxLength(32);
        builder.Property(s => s.Gpa).HasPrecision(4, 2);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Faculty)
            .WithMany()
            .HasForeignKey(s => s.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Department)
            .WithMany()
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class StudentDepartmentHistoryConfiguration : IEntityTypeConfiguration<StudentDepartmentHistory>
{
    public void Configure(EntityTypeBuilder<StudentDepartmentHistory> builder)
    {
        builder.HasQueryFilter(h => h.Student.User.DeletedAt == null && h.SwitchedByUser.DeletedAt == null);

        builder.HasIndex(h => h.StudentId);
        builder.HasIndex(h => h.ToDepartmentId);
        builder.HasIndex(h => h.SwitchedBy);

        builder.Property(h => h.Note).HasMaxLength(1000);

        builder.HasOne(h => h.Student)
            .WithMany(s => s.DepartmentHistory)
            .HasForeignKey(h => h.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.FromDepartment)
            .WithMany()
            .HasForeignKey(h => h.FromDepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(h => h.ToDepartment)
            .WithMany()
            .HasForeignKey(h => h.ToDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.SwitchedByUser)
            .WithMany()
            .HasForeignKey(h => h.SwitchedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
