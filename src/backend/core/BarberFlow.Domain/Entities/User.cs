using BarberFlow.Domain.Common;
using BarberFlow.Domain.ValueObjects;

namespace BarberFlow.Domain.Entities;

public sealed class User : Entity
{
    private User()
    {
    }

    public User(
        string fullName,
        Email email,
        PhoneNumber phoneNumber,
        Cpf cpf)
        : base()
    {
        FullName = DomainGuard.Required(fullName, nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        IsActive = true;
    }

    public string FullName { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    public PhoneNumber PhoneNumber { get; private set; } = null!;

    public Cpf Cpf { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public void UpdateProfile(
        string fullName,
        Email email,
        PhoneNumber phoneNumber,
        Cpf cpf)
    {
        FullName = DomainGuard.Required(fullName, nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        MarkAsUpdated();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        MarkAsUpdated();
    }
}
