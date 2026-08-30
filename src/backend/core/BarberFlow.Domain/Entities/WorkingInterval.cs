using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class WorkingInterval : Entity
{
    private WorkingInterval()
    {
    }

    public WorkingInterval(
        Guid employeeId,
        DayOfWeek dayOfWeek,
        TimeOnly startsAt,
        TimeOnly endsAt)
        : base()
    {
        if (startsAt >= endsAt)
        {
            throw new ArgumentException("The start time must be before the end time.", nameof(startsAt));
        }

        EmployeeId = DomainGuard.Required(employeeId, nameof(employeeId));
        DayOfWeek = dayOfWeek;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public Guid EmployeeId { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    public TimeOnly StartsAt { get; private set; }

    public TimeOnly EndsAt { get; private set; }

}
