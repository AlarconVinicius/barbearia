using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

internal sealed class EmployeeServiceConfiguration : EntityConfiguration<EmployeeService>
{
    protected override void ConfigureEntity(EntityTypeBuilder<EmployeeService> builder)
    {
        builder.ToTable("employee_services");

        builder.Property(employeeService => employeeService.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(employeeService => employeeService.ServiceId)
            .HasColumnName("service_id")
            .IsRequired();

        builder.HasIndex(employeeService => new
        {
            employeeService.EmployeeId,
            employeeService.ServiceId
        })
            .IsUnique()
            .HasDatabaseName("ux_employee_services_employee_id_service_id");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(employeeService => employeeService.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(employeeService => employeeService.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
