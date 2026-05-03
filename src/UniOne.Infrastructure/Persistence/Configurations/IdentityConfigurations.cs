using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniOne.Domain.Entities;

namespace UniOne.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasQueryFilter(u => u.DeletedAt == null);
        builder.HasIndex(u => u.NationalId).IsUnique();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.NationalId).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.Gender).HasConversion<int>();
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.MustChangePassword).HasDefaultValue(false);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Label).IsRequired().HasMaxLength(100);
    }
}

public class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    public void Configure(EntityTypeBuilder<RoleAssignment> builder)
    {
        builder.HasQueryFilter(ra => ra.User.DeletedAt == null);
        builder.HasIndex(ra => new { ra.UserId, ra.RoleId });

        builder.HasOne(ra => ra.User)
            .WithMany(u => u.RoleAssignments)
            .HasForeignKey(ra => ra.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ra => ra.Role)
            .WithMany(r => r.RoleAssignments)
            .HasForeignKey(ra => ra.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PersonalAccessTokenConfiguration : IEntityTypeConfiguration<PersonalAccessToken>
{
    public void Configure(EntityTypeBuilder<PersonalAccessToken> builder)
    {
        builder.HasQueryFilter(ut => ut.User.DeletedAt == null);
        builder.Property(ut => ut.Name).IsRequired().HasMaxLength(100);
        builder.Property(ut => ut.TokenHash).IsRequired().HasMaxLength(256);

        builder.HasOne(ut => ut.User)
            .WithMany()
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
