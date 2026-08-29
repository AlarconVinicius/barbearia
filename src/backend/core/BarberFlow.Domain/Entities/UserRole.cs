using BarberFlow.Domain.Common;
using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class UserRole : Entity
{
    private UserRole()
    {
    }

    public UserRole(Guid userId, UserRoleType role)
        : base()
    {
        UserId = DomainGuard.Required(userId, nameof(userId));
        Role = role;
    }

    public Guid UserId { get; private set; }

    public UserRoleType Role { get; private set; }

}
