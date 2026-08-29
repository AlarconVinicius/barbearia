using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class EmployeeService : Entity
{
    private EmployeeService()
    {
    }

    public EmployeeService(Guid employeeId, Guid serviceId)
        : base()
    {
        EmployeeId = DomainGuard.Required(employeeId, nameof(employeeId));
        ServiceId = DomainGuard.Required(serviceId, nameof(serviceId));
    }

    public Guid EmployeeId { get; private set; }

    public Guid ServiceId { get; private set; }

}
