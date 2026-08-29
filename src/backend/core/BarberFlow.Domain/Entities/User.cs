using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Entities;

public sealed class User : Entity
{
    private User()
    {
    }

    public User(
        string fullName,
        string email,
        string phoneNumber,
        string? cpf = null)
        : base()
    {
        FullName = DomainGuard.Required(fullName, nameof(fullName));
        Email = DomainGuard.Required(email, nameof(email)).ToLowerInvariant();
        PhoneNumber = DomainGuard.Required(phoneNumber, nameof(phoneNumber));
        Cpf = string.IsNullOrWhiteSpace(cpf) ? null : cpf.Trim();
        IsActive = true;
    }

    public string FullName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = null!;

    public string? Cpf { get; private set; }

    public bool IsActive { get; private set; }

    public void UpdateProfile(
        string fullName,
        string email,
        string phoneNumber,
        string? cpf)
    {
        FullName = DomainGuard.Required(fullName, nameof(fullName));
        Email = DomainGuard.Required(email, nameof(email)).ToLowerInvariant();
        PhoneNumber = DomainGuard.Required(phoneNumber, nameof(phoneNumber));
        Cpf = string.IsNullOrWhiteSpace(cpf) ? null : cpf.Trim();
        MarkAsUpdated();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        MarkAsUpdated();
    }
}
