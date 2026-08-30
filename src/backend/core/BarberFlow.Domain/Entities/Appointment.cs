using BarberFlow.Domain.Common;
using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class Appointment : Entity
{
    private Appointment()
    {
    }

    public Appointment(
        Guid customerId,
        Guid employeeId,
        DateTimeOffset startsAtUtc,
        DateTimeOffset endsAtUtc,
        Guid createdByUserId) : base()
    {
        if (customerId == employeeId)
        {
            throw new ArgumentException("The customer and employee must be different users.");
        }

        if (startsAtUtc >= endsAtUtc)
        {
            throw new ArgumentException("The start time must be before the end time.", nameof(startsAtUtc));
        }

        CustomerId = DomainGuard.Required(customerId, nameof(customerId));
        EmployeeId = DomainGuard.Required(employeeId, nameof(employeeId));
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Status = AppointmentStatus.Scheduled;
        CreatedByUserId = DomainGuard.Required(createdByUserId, nameof(createdByUserId));
    }

    public Guid CustomerId { get; private set; }

    public Guid EmployeeId { get; private set; }

    public DateTimeOffset StartsAtUtc { get; private set; }

    public DateTimeOffset EndsAtUtc { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public Guid? CancelledByUserId { get; private set; }

    public string? CancellationReason { get; private set; }

    public void Cancel(Guid cancelledByUserId, string? reason = null)
    {
        if (Status == AppointmentStatus.Cancelled)
        {
            throw new InvalidOperationException("The appointment is already cancelled.");
        }

        CancelledByUserId = DomainGuard.Required(cancelledByUserId, nameof(cancelledByUserId));
        CancelledAtUtc = MarkAsUpdated();
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        Status = AppointmentStatus.Cancelled;
    }
}
