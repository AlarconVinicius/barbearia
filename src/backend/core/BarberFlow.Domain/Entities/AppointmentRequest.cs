using BarberFlow.Domain.Common;
using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class AppointmentRequest : Entity
{
    private AppointmentRequest()
    {
    }

    public AppointmentRequest(
        string idempotencyKey,
        Guid requestedByUserId,
        Guid customerId,
        Guid employeeId,
        DateTimeOffset requestedStartsAtUtc,
        AppointmentRequestType type)
        : base()
    {
        if (customerId == employeeId)
        {
            throw new ArgumentException("The customer and employee must be different users.");
        }

        IdempotencyKey = DomainGuard.Required(idempotencyKey, nameof(idempotencyKey));
        RequestedByUserId = DomainGuard.Required(requestedByUserId, nameof(requestedByUserId));
        CustomerId = DomainGuard.Required(customerId, nameof(customerId));
        EmployeeId = DomainGuard.Required(employeeId, nameof(employeeId));
        RequestedStartsAtUtc = requestedStartsAtUtc;
        Type = type;
        Status = AppointmentRequestStatus.Pending;
    }

    public string IdempotencyKey { get; private set; } = null!;

    public Guid RequestedByUserId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid EmployeeId { get; private set; }

    public DateTimeOffset RequestedStartsAtUtc { get; private set; }

    public AppointmentRequestType Type { get; private set; }

    public AppointmentRequestStatus Status { get; private set; }

    public AppointmentRequestRejectionReason? RejectionReason { get; private set; }

    public string? RejectionDetails { get; private set; }

    public Guid? AppointmentId { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public void Accept(Guid appointmentId)
    {
        EnsurePending();
        AppointmentId = DomainGuard.Required(appointmentId, nameof(appointmentId));
        Status = AppointmentRequestStatus.Accepted;
        ProcessedAtUtc = MarkAsUpdated();
    }

    public void Reject(
        AppointmentRequestRejectionReason reason,
        string? details = null)
    {
        EnsurePending();
        Status = AppointmentRequestStatus.Rejected;
        RejectionReason = reason;
        RejectionDetails = string.IsNullOrWhiteSpace(details) ? null : details.Trim();
        ProcessedAtUtc = MarkAsUpdated();
    }

    private void EnsurePending()
    {
        if (Status != AppointmentRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only a pending appointment request can be processed.");
        }
    }
}
