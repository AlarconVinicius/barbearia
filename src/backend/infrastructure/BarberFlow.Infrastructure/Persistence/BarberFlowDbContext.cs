using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence;

public sealed class BarberFlowDbContext(DbContextOptions<BarberFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<AuthenticationCode> AuthenticationCodes => Set<AuthenticationCode>();

    public DbSet<Service> Services => Set<Service>();

    public DbSet<EmployeeService> EmployeeServices => Set<EmployeeService>();

    public DbSet<WorkingInterval> WorkingIntervals => Set<WorkingInterval>();

    public DbSet<AppointmentRequest> AppointmentRequests => Set<AppointmentRequest>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<AppointmentItem> AppointmentItems => Set<AppointmentItem>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberFlowDbContext).Assembly);
    }
}
