using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Infrastructure.Persistence.Configurations;

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.Property(u => u.Name).IsRequired().HasMaxLength(255);
        builder.Property(u => u.NameAr).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Address).IsRequired();
        builder.Property(u => u.LogoPath).HasMaxLength(255);
        builder.Property(u => u.Phone).HasMaxLength(255);
        builder.Property(u => u.Email).HasMaxLength(255);
        builder.Property(u => u.Website).HasMaxLength(255);
    }
}

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasIndex(f => f.Code).IsUnique();

        builder.Property(f => f.Name).IsRequired().HasMaxLength(255);
        builder.Property(f => f.NameAr).IsRequired().HasMaxLength(255);
        builder.Property(f => f.Code).IsRequired().HasMaxLength(255);
        builder.Property(f => f.LogoPath).HasMaxLength(255);
        builder.Property(f => f.EnrollmentType)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<EnrollmentType>(v, true))
            .HasMaxLength(32);
        builder.Property(f => f.IsActive).HasDefaultValue(true);

        builder.HasOne(f => f.Dean)
            .WithMany()
            .HasForeignKey(f => f.DeanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasIndex(d => d.Code).IsUnique();

        builder.Property(d => d.Name).IsRequired().HasMaxLength(255);
        builder.Property(d => d.NameAr).IsRequired().HasMaxLength(255);
        builder.Property(d => d.Code).IsRequired().HasMaxLength(255);
        builder.Property(d => d.LogoPath).HasMaxLength(255);
        builder.Property(d => d.Type)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<DepartmentType>(v, true))
            .HasMaxLength(32);
        builder.Property(d => d.Scope)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<DepartmentScope>(v, true))
            .HasMaxLength(32)
            .HasDefaultValue(DepartmentScope.Faculty);
        builder.Property(d => d.IsPreparatory).HasDefaultValue(false);
        builder.Property(d => d.IsActive).HasDefaultValue(true);
        builder.Property(d => d.IsMandatory).HasDefaultValue(false);

        builder.HasOne(d => d.Faculty)
            .WithMany(f => f.Departments)
            .HasForeignKey(d => d.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Head)
            .WithMany()
            .HasForeignKey(d => d.HeadId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class UniversityVicePresidentConfiguration : IEntityTypeConfiguration<UniversityVicePresident>
{
    public void Configure(EntityTypeBuilder<UniversityVicePresident> builder)
    {
        builder.HasIndex(vp => vp.ProfessorId).IsUnique();

        builder.Property(vp => vp.Title).IsRequired().HasMaxLength(255);
        builder.Property(vp => vp.TitleAr).IsRequired().HasMaxLength(255);
        builder.Property(vp => vp.Order).HasDefaultValue((byte)0);
        builder.Property(vp => vp.IsActive).HasDefaultValue(true);

        builder.HasOne(vp => vp.University)
            .WithMany(u => u.VicePresidents)
            .HasForeignKey(vp => vp.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
