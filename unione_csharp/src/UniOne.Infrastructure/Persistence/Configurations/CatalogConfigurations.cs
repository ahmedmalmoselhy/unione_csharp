using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Infrastructure.Persistence.Configurations;

public class AcademicTermConfiguration : IEntityTypeConfiguration<AcademicTerm>
{
    public void Configure(EntityTypeBuilder<AcademicTerm> builder)
    {
        builder.HasIndex(t => new { t.AcademicYear, t.Semester }).IsUnique();

        builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
        builder.Property(t => t.NameAr).HasMaxLength(255);
        builder.Property(t => t.AcademicYear).IsRequired().HasMaxLength(32);
        builder.Property(t => t.Semester)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<Semester>(v, true))
            .HasMaxLength(32);
        builder.Property(t => t.IsActive).HasDefaultValue(true);
    }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Code).IsRequired().HasMaxLength(255);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(255);
        builder.Property(c => c.NameAr).HasMaxLength(255);
        builder.Property(c => c.Description).HasMaxLength(2000);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
    }
}

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasQueryFilter(s => s.Professor.User.DeletedAt == null);

        builder.HasIndex(s => s.CourseId);
        builder.HasIndex(s => s.ProfessorId);
        builder.HasIndex(s => s.AcademicTermId);
        builder.HasIndex(s => new { s.CourseId, s.AcademicTermId, s.ProfessorId });

        builder.Property(s => s.Room).HasMaxLength(255);
        builder.Property(s => s.Schedule).HasColumnType("jsonb");
        builder.Property(s => s.IsActive).HasDefaultValue(true);

        builder.HasOne(s => s.Course)
            .WithMany(c => c.Sections)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Professor)
            .WithMany()
            .HasForeignKey(s => s.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.AcademicTerm)
            .WithMany(t => t.Sections)
            .HasForeignKey(s => s.AcademicTermId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
