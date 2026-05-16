using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniOne.Domain.Entities;
using UniOne.Domain.Enums;

namespace UniOne.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasQueryFilter(e => e.Student.User.DeletedAt == null && e.Section.Professor.User.DeletedAt == null);

        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.SectionId);
        builder.HasIndex(e => e.AcademicTermId);
        builder.HasIndex(e => new { e.StudentId, e.SectionId }).IsUnique();

        builder.Property(e => e.Status)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<EnrollmentRecordStatus>(v, true))
            .HasMaxLength(32);

        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Section)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.AcademicTerm)
            .WithMany(t => t.Enrollments)
            .HasForeignKey(e => e.AcademicTermId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EnrollmentWaitlistConfiguration : IEntityTypeConfiguration<EnrollmentWaitlist>
{
    public void Configure(EntityTypeBuilder<EnrollmentWaitlist> builder)
    {
        builder.HasQueryFilter(w => w.Student.User.DeletedAt == null && w.Section.Professor.User.DeletedAt == null);

        builder.HasIndex(w => w.StudentId);
        builder.HasIndex(w => w.SectionId);
        builder.HasIndex(w => w.AcademicTermId);
        builder.HasIndex(w => new { w.StudentId, w.SectionId }).IsUnique();
        builder.HasIndex(w => new { w.SectionId, w.Position }).IsUnique();

        builder.HasOne(w => w.Student)
            .WithMany()
            .HasForeignKey(w => w.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Section)
            .WithMany(s => s.Waitlists)
            .HasForeignKey(w => w.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.AcademicTerm)
            .WithMany()
            .HasForeignKey(w => w.AcademicTermId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
