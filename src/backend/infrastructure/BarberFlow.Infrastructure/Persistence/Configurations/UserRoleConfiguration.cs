using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class UserRoleConfiguration : EntityConfiguration<UserRole>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.Property(userRole => userRole.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(userRole => userRole.Role)
            .HasColumnName("role")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(userRole => new { userRole.UserId, userRole.Role })
            .IsUnique()
            .HasDatabaseName("ux_user_roles_user_id_role");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(userRole => userRole.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
