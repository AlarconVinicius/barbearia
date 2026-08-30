using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class AppointmentItem : Entity
{
    private AppointmentItem()
    {
    }

    public AppointmentItem(
        Guid appointmentId,
        Guid serviceId,
        string serviceName,
        decimal unitPrice,
        int durationMinutes)
        : base()
    {
        AppointmentId = DomainGuard.Required(appointmentId, nameof(appointmentId));
        ServiceId = DomainGuard.Required(serviceId, nameof(serviceId));
        ServiceName = DomainGuard.Required(serviceName, nameof(serviceName));
        UnitPrice = DomainGuard.NonNegative(unitPrice, nameof(unitPrice));
        DurationMinutes = DomainGuard.Positive(durationMinutes, nameof(durationMinutes));
    }

    public Guid AppointmentId { get; private set; }

    public Guid ServiceId { get; private set; }

    public string ServiceName { get; private set; } = null!;

    public decimal UnitPrice { get; private set; }

    public int DurationMinutes { get; private set; }
}
